using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace EQBrowser {

	public partial class WorldConnect : MonoBehaviour
	{  
		private static WorldConnect instance;
		public GameObject WorldConnectObject;
		public GameObject[] SpawnRaceId;
		public GameObject CharSelect;
		public GameObject CharSelectCamera;
		public GameObject UIScriptsObject;
		public UIScripts UIScript;
		public GameObject GameCamera;
		public GameObject ChatText;
		public GameObject EqemuConnectObject;
		public CharacterSelect CSel;
		public Text ChatText2;
		public GameObject NullGameObject;
		public static WorldConnect Instance
		{
			get
			{
				
				if (instance == null)
				{
					instance = new WorldConnect();
				}
				
				return instance;
			}
		}

		
		public void Awake() {
			DontDestroyOnLoad(this);
		}

		
		public bool Connected;
		public bool AttemptingZoneConnect;
		public bool isTyping;
		public string ourPlayerName;
		public Int32 curZoneId = -1;
		public Int32 curInstanceId = -1;
		public Int32 OurEntityID = 0;
		public WebSocket ws_;

		delegate void OpcodeFunc (byte[] data, int datasize);
		//Use this for initialization
		
		public Byte ReadInt8(byte[] data, ref Int32 position)
		{
			byte retval = data[position];
			position += 1;
			return retval;
		}

		public Int16 ReadInt16(byte[] data, ref Int32 position)
		{
			Int16 retval = BitConverter.ToInt16(data, position);
			position += 2;
			return retval;
		}

		public Int32 ReadInt32(byte[] data, ref Int32 position)
		{
			Int32 retval = BitConverter.ToInt32 (data, position);
			position += 4;
			return retval;
		}

		public string ReadFixedLengthString(byte[] data, ref Int32 position, Int32 count)
		{
			string retval = System.Text.Encoding.Default.GetString(data, position, count);
			position += count;
			return retval;
		}

		public byte[] ReadFixedLengthByteArray(byte[] data, ref Int32 position, Int32 count)
		{
			byte[] retval = new byte[count];
			Buffer.BlockCopy (data, position, retval, 0, count);
			position += count;
			return retval;
		}
		
		
		public void WriteInt8(byte value, ref byte[] data, ref Int32 position)
		{
			Buffer.BlockCopy (BitConverter.GetBytes(value), 0, data, position, 1);
			position += 1;
			return;
		}
		
		public void WriteInt16(Int16 value, ref byte[] data, ref Int32 position)
		{
			Buffer.BlockCopy (BitConverter.GetBytes(value), 0, data, position, 2);
			position += 2;
			return;
		}
		
		public void WriteInt32(Int32 value, ref byte[] data, ref Int32 position)
		{
			Buffer.BlockCopy (BitConverter.GetBytes(value), 0, data, position, 4);
			position += 4;
			return;
		}
		
		public void WriteFloat(float value, ref byte[] data, ref Int32 position)
		{
			Buffer.BlockCopy (BitConverter.GetBytes(value), 0, data, position, 4);
			position += 4;
			return;
		}
		
		public void WriteFixedLengthString(string inString, ref byte[] data, ref Int32 position, Int32 count)
		{
			Buffer.BlockCopy (Encoding.UTF8.GetBytes(inString), 0, data, position, Encoding.UTF8.GetBytes(inString).Length);
			position += count;
			return;
		}
		
		public void WriteFixedLengthByteArray(byte[] inArray, ref byte[] data, ref Int32 position, Int32 count)
		{
			Buffer.BlockCopy (inArray, 0, data, position, inArray.Length);
			position += count;
			return;
		}

		Dictionary<string, OpcodeFunc> opcodeDict;

		void Start()
	    {
			instance = this;

			opcodeDict = new Dictionary<string, OpcodeFunc>();//for testing
			opcodeDict.Add ("423", HandleWorldMessage_SendCharInfo);
			opcodeDict.Add ("36", HandleWorldMessage_ApproveName);
			opcodeDict.Add ("548", HandleWorldMessage_ZoneUnavailable);
			opcodeDict.Add ("545", HandleWorldMessage_ZoneServerInfo);
			opcodeDict.Add ("365", HandleWorldMessage_PlayerProfile);
			opcodeDict.Add ("338", HandleWorldMessage_NewZone);
			opcodeDict.Add ("546", HandleWorldMessage_ZoneServerReady);
			opcodeDict.Add ("549", HandleWorldMessage_EmuKeepAlive);
			opcodeDict.Add ("551", HandleWorldMessage_EmuRequestClose);
			opcodeDict.Add ("296", HandleWorldMessage_LogOutReply);
			opcodeDict.Add ("424", HandleWorldMessage_SendExpZonein);
			opcodeDict.Add ("547", HandleWorldMessage_ZoneSpawns);
			opcodeDict.Add ("336", HandleWorldMessage_ZoneSpawns);
			opcodeDict.Add ("539", HandleWorldMessage_ZoneChange);
			opcodeDict.Add ("116", HandleWorldMessage_DeleteSpawn);
			opcodeDict.Add ("69", HandleWorldMessage_ChannelMessage);
			opcodeDict.Add ("87", HandleWorldMessage_ClientUpdate);
			opcodeDict.Add ("458", HandleWorldMessage_SimpleMessage);


			//Auto-Connect to Salty Server
//			StartCoroutine(ConnectToWebSocketServer("158.69.221.200", "aksdjlka23ij3l1j23lk1j23j123jkjql", "XLOGINX", "XPASSWORDX"));

	    }

	    public class Auth
	    {
	        public string id;
	        public string method;
	        public string[] @params;
		}

		[Serializable]
		public class OpcodeFromServerClass
		{
			public string id;
			public string method;
			public string zoneid;
			public string instanceid;
			public string opcode;
			public string datasize;
			public string data;
		}

		[Serializable]
		public class OpcodeToServerClass
		{
			public string id;
			public string method;
			public string[] @params;
			
		}

		[Serializable]		
	    public class CheckIdClass
	    {
	        public static string id;
	        public static string method;
	        public string[] @params;
		}
	
		public IEnumerator ConnectToWebSocketServer(string hostname, string auth, string username, string password)
		{

			if(ws_ != null)
			{
				ws_.Close();
				ws_ = null;
			}

//			string realhost = "ws://" + "158.69.221.200"  + ":80";
//			string realhost = "ws://" + hostname+ ":9080";
			string realhost = "ws://" + hostname;			
			ws_ = new WebSocket(new Uri(realhost));
			yield return StartCoroutine(ws_.Connect());

			Auth auth1 = new Auth();
			auth1.id = "token_auth_id";
			auth1.method = "WebInterface.Authorize";
			auth1.@params = new string[1] { auth };

			string authoutput =  JsonUtility.ToJson(auth1);
			ws_.SendString(authoutput);
			Debug.Log("SendAuth: " + authoutput);
			
			//for testing, build username portion
			byte[] userNamePass = new byte[64];
			Buffer.BlockCopy(Encoding.UTF8.GetBytes(username), 0, userNamePass, 0, username.Length);
			Buffer.BlockCopy(Encoding.UTF8.GetBytes(password), 0, userNamePass, username.Length + 1, Encoding.UTF8.GetBytes(password).Length);
			byte[] Zoning = new byte[1];
			Zoning[0] = 0;
			GenerateAndSendWorldPacket (65, 427, -1, -1, userNamePass, Zoning);
			
			Connected = true;
			yield return 0;
		} 
		
		//This is called assuming we have a connection.
		public string GenerateWorldPacket(int pktsize, short opcode, Int32 zoneid, Int32 instanceid, params byte[][] list)
		{

			byte[] OutPkt = new byte[0];

			if(pktsize > 0)
				OutPkt = new byte[pktsize];

			int cur_pos = 0;

			if (OutPkt.Length > 0) {
				for (int i = 0; i < list.Length; i++) {
					Buffer.BlockCopy (list [i], 0, OutPkt, cur_pos, list [i].Length);
					cur_pos += list [i].Length;
				}
			}
			string base64Packet = "";
			base64Packet = System.Convert.ToBase64String (OutPkt);
			OpcodeToServerClass SerializedPacket = new OpcodeToServerClass();
			SerializedPacket.id = "1337";
			SerializedPacket.method = "World.OpcodeToClient";
			SerializedPacket.@params = new string[5] { zoneid.ToString(), instanceid.ToString (), opcode.ToString(), cur_pos.ToString(), base64Packet };

			string SerializedPacketString = JsonUtility.ToJson(SerializedPacket);
			return SerializedPacketString;


			//Request Entity Positions from server
		}

		public void GenerateAndSendWorldPacket(int pktsize, short opcode, Int32 zoneid, Int32 instanceid, params byte[][] list)
		{
			string serialized = GenerateWorldPacket(pktsize, opcode, zoneid, instanceid, list);
			ws_.SendString(serialized);
			//Debug.Log("SendPacket" + serialized);
		}

		// Update is called once per frame
		void Update ()
		{

			//Establish a world connection.
			if (ws_ != null) {
				string reply = ws_.RecvString ();
				if (reply != null) {
					
					//Debug.Log("reply" + reply);
					OpcodeFromServerClass IdChecker1 = JsonUtility.FromJson<OpcodeFromServerClass> (reply);

					if (IdChecker1.id != null && IdChecker1.id == "token_auth_id") {
						if(CSel != null)
							CSel.LoginStatus.text = "Auth successful. Proceeding...";
					}
					if (IdChecker1.method == "Client.Opcode") {
						byte[] RawData = null;
						if (Convert.ToInt32(IdChecker1.datasize) > 0) {
							RawData = System.Convert.FromBase64CharArray (IdChecker1.data.ToCharArray(), 0, IdChecker1.data.Length);
						}

						if(opcodeDict.ContainsKey(IdChecker1.opcode)){
							string RawOp = IdChecker1.opcode;

							int length = 0;
							if(RawData != null)
								length = RawData.Length;
							opcodeDict[RawOp](RawData, length);
						}
					}
				}
				if (ws_.Error != null) {
					LogError ("Error: " + ws_.Error);
					ws_ = null;
					ws_.Close ();
					SceneManager.LoadScene("1 Character creation");
				}
			}
		}

		public void Log(string message)
		{
			Debug.Log(message);
			Application.ExternalCall("console.log", message);
		}
		
		public void LogError(string error)
		{
			Debug.Log(error);
			Application.ExternalCall("console.log", error);
		}
	}

}