// --------------------------------------------------------------------------------------------------------------------
// <copyright file="usePhoton.cs" company="Exit Games GmbH">
// Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
// This is the "main" script of the Photon Bootcamp Loadbalancing Demo for Unity.
// This sample should show how to get a player's position across to other players in the same room/game.
// Attach to a camera (or GameObject) to update the game and players.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// This class is the intersection between the game logic (in class game) and Unity. Attached to a GameObject (cam) 
// this will update the game and process input. 
// This sample should show how to get a player's position across to other players in the same room.
// Each running instance will connect to PhotonCloud (with a local player / peer), go into the Lobby and 
// show the user the available rooms/games. 
//
// The actual handling of Photon is done in the Game and Player classes.

#if UNITY_EDITOR

using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CoverScreen : MonoBehaviour
{
    public bool MainStart = true;
   // private bool nameEntered; // used to show logos and name-input
	//zc1415926
	public string nextSceneName;

    public bool logo1 = true;
    public bool logo2 = false;

	//zc1415926
	public Texture2D FirstLogo;
	public int FirstLogoWidth = 0;
	public int FirstLogoHeight = 0;

    //public Texture2D PhotonLogo;
	//public int PhotonLogoWidth = 0;
	//public int PhotonLogoHeight = 0;

	public Texture2D SecondLogo;
	public int SecondLogooWidth = 0;
	public int SecondLogoHeight = 0;

	//public Texture2D KingsMoundLogo;
	//public int KingsMoundLogoWidth = 0;
	//public int KingsMoundLogooHeight = 0;

    private float opacity = 0;
    private bool isVisible = false;
    private bool isHiding = false;
    private bool isShowing = false;
	
	private JumpSceneAsyncUtility utils;
	
    void Start()
    {
        Application.targetFrameRate = 30;
        Application.runInBackground = true;     // this is essential to keep connections without having focus.
        StartCoroutine(this.DisplayLogoRutin());
		
		utils = new JumpSceneAsyncUtility();
		//zc1415926
		StartCoroutine(utils.loadSceneAsync(nextSceneName));
		//StartCoroutine(utils.loadSceneAsync("MainMenu"));
        //this.StartGame();
    }

    IEnumerator DisplayLogoRutin()
    {
        if (!MainStart)
        {
			yield return new WaitForSeconds(1);
            Show();
            yield return new WaitForSeconds(3);
            Hide();
            yield return new WaitForSeconds(1);
            logo1 = false;

            yield return new WaitForSeconds(1);
            logo2 = true;
            Show();
            yield return new WaitForSeconds(3);

            Hide();
            yield return new WaitForSeconds(1);
            logo2 = false;

            MainStart = true;
        }
    }

    public void DisplayLogo(Texture2D Logo)
    {
        GUI.DrawTexture(new Rect((int)((Screen.width / 2) - (Logo.width / 2)), (int)((Screen.height / 2) - (Logo.height / 2)), Logo.width, Logo.height), Logo);
    }

    public void DisplayLogo(Texture2D Logo, float height, float width)
    {
        GUI.DrawTexture(new Rect((int)((Screen.width / 2) - (width / 2)), (int)((Screen.height / 2) - (height / 2)), width, height), Logo);
    }

    public void OnGUI()
    {
        //if (!this.nameEntered)
       // {
            Color oldColor;
            Color auxColor;
            oldColor = auxColor = GUI.color;

            if (logo1)
            {
                auxColor.a = opacity;
                GUI.color = auxColor;
                DisplayLogo(FirstLogo, FirstLogoHeight, FirstLogoWidth);
            }

            if (logo2)
            {
                auxColor.a = opacity;
                GUI.color = auxColor;
                DisplayLogo(SecondLogo, SecondLogoHeight, SecondLogooWidth);
            }
            GUI.color = oldColor;

            if (MainStart)
            {
				/*
				 * 从这里结束片头，执行转场 
				 * 也可以点击鼠标直接转卖
				 */
				utils.jumpLevel();
				Debug.Log("Start!");
            
            }
            //return;
        //}

    }

    public void Show()
    {
        if (isVisible == false)
        {
            StartCoroutine(ShowBySlideFromBottom());
        }
    }

    public void Hide()
    {
        if (isVisible && !isHiding)
        {
            StartCoroutine(HideByFading());
        }
    }

    private IEnumerator ShowBySlideFromBottom()
    {
        if (!isShowing || !isHiding)
        {
            isShowing = isVisible = true; opacity = 0;
            while (opacity < 1)
            {
                opacity += Time.deltaTime * 2f;
                yield return new WaitForEndOfFrame();
            }
            opacity = 1; isShowing = false;
        }
    }

    private IEnumerator HideByFading()
    {
        if (!isShowing || !isHiding)
        {
            isHiding = true;

            while (opacity > 0)
            {
                opacity -= Time.deltaTime * 3;
                yield return new WaitForEndOfFrame();
            }
            if (opacity <= 0)
            {
                if (logo1) logo1 = false;
                if (logo2) logo2 = false;
            }
            isHiding = isVisible = false; opacity = 1;
        }
    }
}