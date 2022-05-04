﻿using System.Collections.Generic;
using Mechanics.Feedback;
using UnityEngine;
using UnityEngine.UI;

public class Journal : MonoBehaviour
{
    public Tab[] tabs = new Tab[7];

    public List<Page> pages = new List<Page>();

    [SerializeField] private Page activePage, pausePage = null, lastPage = null;

    public Button nextBtn = null, previousBtn = null;

    [SerializeField] private SfxUiLibrary _sfxUiLibrary = null;
    public SfxUiLibrary SfxUiLibrary => _sfxUiLibrary;

    private int tabIndex;

    private void Start()
    {
        tabs[0].associatedPage = 0;
        tabs[1].associatedPage = 1;
        tabs[2].associatedPage = 2; 
        tabs[3].associatedPage = 3;
        tabs[4].associatedPage = 4;
        tabs[5].associatedPage = 5;
        tabs[6].associatedPage = 7;

        //Gives each page a list identifier
        int i = 0;
        foreach (Page page in pages)
        {
            page.index = i;
            i++;
        }
        ActivatePage(pausePage);
    }

    public void NextPage()
    {
        int currentIndex = activePage.index;
        if (_sfxUiLibrary != null) _sfxUiLibrary.OnJournalPageRight();
        ActivatePage(currentIndex + 1);
    }

    public void PreviousPage()
    {
        int currentIndex = activePage.index;
        if (_sfxUiLibrary != null) _sfxUiLibrary.OnJournalPageLeft();
        ActivatePage(currentIndex - 1);
    }

    

    public void ActivatePage(int pageNum)
    {
        if (activePage != null)
            activePage.gameObject.SetActive(false);

        activePage = pages[pageNum];
        activePage.gameObject.SetActive(true);
        UpdateTabs();
    }

    public void ActivatePage(Page page)
    {
        if (activePage != null)
            activePage.gameObject.SetActive(false);

        activePage = page;
        activePage.gameObject.SetActive(true);
        UpdateTabs();
    }

    private void OnEnable()
    {
        ActivatePage(pausePage);
    }

    //When UpdateTabs() is called, all tabs should update relative to the what the current ActivePage is
    public void UpdateTabs()
    {
        foreach (Tab tab in tabs)
        {
            Page tabPage = pages[tab.associatedPage];
            if (activePage.index < tabPage.index)
            {
                tab.ResetPosition();
            }
            else
            {
                if (tabPage == activePage) tab.ChangePosition(true);
                else tab.ChangePosition(false);
            }    
        }
    }

    private void Update()
    {
        if (activePage == pausePage)
        {
            previousBtn.gameObject.SetActive(false);
        }
        else if (activePage == lastPage)
        {
            nextBtn.gameObject.SetActive(false);
        }
        else
        {
            nextBtn.gameObject.SetActive(true);
            previousBtn.gameObject.SetActive(true);
        }
    }
}
