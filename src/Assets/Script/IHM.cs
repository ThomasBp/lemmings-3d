using UnityEngine;
using System.Collections;

public class IHM : MonoBehaviour {
	
	const int MARGIN = 5;
	const int BUTTON_WIDTH = 80;
	const int BUTTON_HEIGHT = 80;
	const int BUTTON_NUMBER = 8;
	
	const int SCORE_LEMMING_WIN = 10;
	const int SCORE_LEMMING_DEAD = -5;
	const float SCORE_BY_SECOND = 1F;
	
	public static Lemming.ACTION action = Lemming.ACTION.DEFAULT;
	
	private static int lemmingWin = 0;
	private static int lemmingDead = 0;
	
	public int timeMax;
	
	private int _i;
	private int _left;
	
	private int _barWidth;
	private int _barHeight;
	
	private int _btWidth;
	private int _btHeight;
	
	///  ----------------------
	/// |  ----   ----   ----   |
	/// | |    | |    | |    |  |- HEIGHT
	/// |  ----   ----   ----   |
	///  ----------------------
	/// <-> MARGIN     <-WIDTH->
	
	// Use this for initialization
	void Start () {
		_btWidth	= MARGIN + BUTTON_WIDTH;
		_btHeight	= MARGIN + BUTTON_HEIGHT;
		_barWidth	= MARGIN + BUTTON_NUMBER * _btWidth;
		_barHeight	= MARGIN + _btHeight;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public static void IFindExit()
	{
		lemmingWin++;
	}
	
	public static void IWillDie()
	{
		lemmingDead++;
	}
	
	private int GetScore()
	{
		return	lemmingWin*SCORE_LEMMING_WIN + lemmingDead*SCORE_LEMMING_DEAD;
	}
	
	// Draw gui()
	void OnGUI()
	{
		GUI.Label(new Rect(MARGIN, MARGIN, 80, 20), "Score:"+GetScore());
		
		_left = (Screen.width-_barWidth)/2;
		GUI.Box(new Rect(_left, Screen.height-_barHeight, _barWidth, _barHeight), "Tool");
		_left += MARGIN;
		_i = 0;
		if(Button("MINE"))		action = Lemming.ACTION.MINE;
		if(Button("EXPLODE"))	action = Lemming.ACTION.EXPLODE;
		if(Button("DIG"))		action = Lemming.ACTION.DIG;
		if(Button("BUILD"))		action = Lemming.ACTION.BUILD;
		if(Button("BLOCK"))		action = Lemming.ACTION.BLOCK;
		if(Button("FLOAT"))		action = Lemming.ACTION.FLOAT;
		if(Button("CLIMB"))		action = Lemming.ACTION.CLIMB;
		if(Button("BASH"))		action = Lemming.ACTION.BASH;
	}
	
	bool Button(string title)
	{
		bool r = GUI.Button(new Rect (_left + _btWidth * _i,
		                              Screen.height - _btHeight,
		                              BUTTON_WIDTH,
		                              BUTTON_HEIGHT),
		                    title);
		_i++;
		return r;
	}
	
}
