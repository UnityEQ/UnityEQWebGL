#pragma strict

var frames : Texture2D[];
var framesPerSecond = 10.0;
 
function Update () {
	var index : int = Time.time * framesPerSecond;
	index = index % frames.Length;
	GetComponent(Renderer).material.mainTexture = frames[index];
}