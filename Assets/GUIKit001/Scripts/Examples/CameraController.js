var animate : boolean = true;


function LateUpdate () {
	if(animate) transform.parent.eulerAngles.y = Mathf.Sin(Time.time * 0.2) * 15;
}