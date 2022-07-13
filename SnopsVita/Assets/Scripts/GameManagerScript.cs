using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //Lokalni igralec je vedno 0, nato pa so vsi drugi
    public PlayFieldScript IgralnoPolje;
    public PlayerScript[] igralci;
    public DeckScript deck;
    public string adut;
    public bool farbanje; //�e je true mora igralec dati ven enako karto
    //public List<Transform> lokacijeIgralcev = new List<Transform>();
    public List<PobraneKarteIgralca> pobraneKarteSkript;
    public int stIgralcev;
    public PlayerScript lokalniIgralec;
    protected TipIgre tipIgre;
    protected int stIgralcaKiSpila;
    public int stTockDoKonca = 7;

    [SerializeField]
    public int stTrenutnegaIgralca = -1;
    protected bool prvaIgra = true;
    [Header("UI")]
    public TMP_Text UI_trenutniIgralec;
    public TMP_Text UI_tockeIgralca;
    public TMP_Text UI_scoreLokalnegaIgr;
    public TMP_Text UI_scoreNasprotnika;
    public SpriteAtlas atlasSimbolov = null;
    public Image UI_SlikaAduta;

    public enum StanjeIgre
    {
        nedejavna,
        izbiraAduta,
        igraZacela,
        rundaZacela,
        rundaPoteka,
        rundaKoncala,
        igraKoncala,
        igraKoncalaCakamOdl,
        igraDoseglaVseTocke
    };

    protected enum TipIgre
    {
        navaden,
        zaprtAdut
    }

    [SerializeField]
    protected StanjeIgre stanjeIgre = StanjeIgre.nedejavna;
    protected void Awake()
    {
        stTockDoKonca = 7;
        stIgralcev = 2;
        Debug.Log("Ustvarjam igralce");
        
        igralci[0].imeIgralca = "LokalniIgralec";
        igralci[0].IDIgralca = "LokalniIgralec";
        //igralci[0].pozicija = lokacijeIgralcev[0].transform.position;
        igralci[0].PobraneKarteSkript = pobraneKarteSkript[0];
        lokalniIgralec = igralci[0];
        for(short i = 1; i < stIgralcev; i++)
        {
            igralci[i].imeIgralca = "Bot "+i;
            igralci[i].IDIgralca = "Igralec"+i;
            //igralci[i].pozicija = lokacijeIgralcev[0].transform.position;
            igralci[i].PobraneKarteSkript = pobraneKarteSkript[(i%2)];
            igralci[i].jeBot = true;
            igralci[i].stIgralca = i;
        }
    }

    public void skrijAliPokaziKarte()
    {
        lokalniIgralec.pokaziKarte();
        for (short i = 0; i < stIgralcev; i++)
        {
            if(igralci[i]!= lokalniIgralec)
                igralci[i].skrijKarte(); //igralci[i].pokaziKarte();
        }    
    }

    public void podeliKarteIgralcu(int indeksIgralca,int stKart)
    {
        deck.podeliKarteIgralcu(igralci[indeksIgralca], stKart);
        skrijAliPokaziKarte();
    }

    protected virtual void naZacetekIgre() //Vsemn igralcem podelimo karte, nastavimo aduta, in posodobimo UI
    {
        Debug.Log("BaseZacetekIgre");
        //if(deck.karte.Count==0)
        //    deck.ustvariDeck();
        switch (stIgralcev)
        {
            case 2:
                {
                    Debug.Log("Stevilo trenutnega igralca je: " + stTrenutnegaIgralca);
                    podeliKarteIgralcu(stTrenutnegaIgralca, 5); //Podelimo karte trenutnemu igralcu
                    if (stTrenutnegaIgralca == 0) //Podelimo karte �e ostlamu igralcu
                        podeliKarteIgralcu(1, 5);
                    else
                        podeliKarteIgralcu(0, 5);
                    adut = deck.ustvariAduta(); //Deck ustavri adut (opmba: ta funkcija velja samo za 2 igralca)
                    nastaviSimbolAduta(); //V UI nastavimo pravilno sliko aduta
                    posodobiInformacijeIgralcev(); //Poskrbimo da so spremenljivke pravilno setane
                    farbanje = false;
                    break;
                }
        }
    }
    public void predvajajIgro()
    {
        Debug.Log(stanjeIgre);
        switch (stanjeIgre)
        {
            case StanjeIgre.igraZacela:
                {
                    Debug.Log("Igra se je zacela");
                    NaIgraZacela();
                    break;
                }
            case StanjeIgre.rundaZacela:
                {
                    Debug.Log("Runda se je zacela");
                    NaNovaRundaZacela();
                    break;
                }
            case StanjeIgre.rundaPoteka:
                {
                    Debug.Log("Runda poteka");
                    NaNaslednjaRundaZacela();
                    break;
                }
            case StanjeIgre.rundaKoncala:
                {
                    Debug.Log("Runda koncala");
                    NaRundaKoncala();
                    break;
                }
            case StanjeIgre.igraKoncala:
                {
                    Debug.Log("Igra koncala");
                    NaIgraKoncala();
                    break;
                }
        }
    }

    protected virtual void NaIgraZacela()
    {
        naslednjiIgralec(); //Izberemo naklju�nega igralca
        deck.ustvariDeck();
        naZacetekIgre(); //Podelimo karte igralcem in nastavimo aduta
        stanjeIgre = StanjeIgre.rundaZacela;
        predvajajIgro();
    }

    public void NaNovaRundaZacela()
    {
        switch (stIgralcev)
        {
            case 2:
                {
                    if (deck.karte.Count == 0)
                        farbanje = true;
                    else if(!farbanje)
                        PreveriStKartIgralcev(5);
                    posodobiInformacijeIgralcev();
                    if (igralci[stTrenutnegaIgralca].jeBot)
                    {
                        GameObject t = CallBotNarediPotezo();
                        if(t!=null)
                            PremakniKartoVIgralnoPolje(t);
                        return;
                        //stanjeIgre = StanjeIgre.rundaPoteka;
                        //predvajajIgro();
                    } 
                    break;
                }
        }
    }

    protected GameObject CallBotNarediPotezo()
    {
        return igralci[stTrenutnegaIgralca].BotNarediPotezo(IgralnoPolje.Karte, stIgralcev, IgralnoPolje.NajKarta(), farbanje);
    }

    public int StKartNaPolju()
    {
        return IgralnoPolje.Karte.Count;
    }
    public int StKartVDecku()
    {
        return deck.karte.Count;
    }
    public void NaNaslednjaRundaZacela()
    {
        switch (stIgralcev)
        {
            case 2:
                {
                    if (deck.karte.Count == 0)
                        farbanje = true;
                    posodobiInformacijeIgralcev();
                    if (igralci[stTrenutnegaIgralca].jeBot)
                    {
                        GameObject t = CallBotNarediPotezo();
                        if (t != null)
                            PremakniKartoVIgralnoPolje(t);
                        return;
                        //stanjeIgre = StanjeIgre.rundaPoteka;
                        //predvajajIgro();
                    }
                    break;
                }
        }
    }

    public void NaRundaKoncala()
    {
        string idIgralca = IgralnoPolje.KateriIgralecPobere();
        /*Debug.Log(idIgralca);*/
        int stKartIgralcev = 0;
        for(int i = 0; i < stIgralcev; i++)
        {
            stKartIgralcev += igralci[i].karteIgralca.Count;
            if (igralci[i].IDIgralca == idIgralca)
            {
                igralci[i].PobraneKarteSkript.SprejemiNoveKarte(IgralnoPolje.Karte);
                soundManager.Play("TurnOver" + (int)UnityEngine.Random.Range(1, 4));
                IgralnoPolje.IzprazniListKart();
                stTrenutnegaIgralca = i;
                if (PreveriCeJeKonecIgre())
                    stanjeIgre = StanjeIgre.igraKoncala;
                else
                    stanjeIgre = StanjeIgre.rundaZacela;
                
                predvajajIgro();
            }
        }
    }

    protected bool PreveriCeJeKonecIgre()
    {
        if (KdoJezmagalIgro() == null)
            return false;
        return true;
    }

    protected PobraneKarteIgralca KdoJezmagalIgro()
    {
        int stKartIgr = 0;
        switch (tipIgre)
        {
            case TipIgre.navaden:
                {
                    for (int i = 0; i < stIgralcev; i++)
                    {
                        stKartIgr += igralci[i].karteIgralca.Count;
                        if (igralci[i].PobraneKarteSkript.tocke > 65)
                            return igralci[i].PobraneKarteSkript;
                    }
                    if (stKartIgr == 0)
                        return igralci[stTrenutnegaIgralca].PobraneKarteSkript;
                    return null;
                }
            case TipIgre.zaprtAdut:
                {
                    if (igralci[stIgralcaKiSpila].PobraneKarteSkript.tocke > 65)
                        return igralci[stIgralcaKiSpila].PobraneKarteSkript;
                    else if (igralci[stIgralcaKiSpila].karteIgralca.Count == 0)
                    {
                        return VrniDrugePobraneKI(igralci[stIgralcaKiSpila].PobraneKarteSkript);
                    }

                    return null;
                }
        }
        return null;
    }

    protected PobraneKarteIgralca VrniDrugePobraneKI(PobraneKarteIgralca pk)
    {
        if (pk == pobraneKarteSkript[0])
            return pobraneKarteSkript[1];
        return pobraneKarteSkript[0];
    }

    public void NaIgraKoncala()
    {
        dodajTockeZmagIgr();
        
        posodobiTextTrenutnegaIgralca();
        if (!preveriCeSoVseTockeDosezene())
        {
            posodobiScoreboard();
            PokaziScore();
            stanjeIgre = StanjeIgre.igraKoncalaCakamOdl;
        }
        else
        {
            stanjeIgre = StanjeIgre.igraDoseglaVseTocke;
            gameOverPopUp.SetActive(true);
            onGameOver();
            MP_UpdateElo();
        }
        IzbrisiVseKarte();
    }
    protected virtual void MP_UpdateElo() { }

    public GameObject gameOverPopUp;
    public TMP_Text gameOverMSGText;

    protected void onGameOver()
    {
        if (lokalniIgralec.PobraneKarteSkript.tockZmage>=stTockDoKonca)
        {
            gameOverMSGText.text = "You won the game!";
        }
        else
        {
            gameOverMSGText.text = "You lost the game";
        }
    }

    protected void posodobiScoreboard()
    {
        UI_scoreLokalnegaIgr.text += lokalniIgralec.PobraneKarteSkript.tockZmage.ToString() + "\n";
        if (lokalniIgralec.PobraneKarteSkript == pobraneKarteSkript[0])
            UI_scoreNasprotnika.text += pobraneKarteSkript[1].tockZmage.ToString() + "\n";
        else
            UI_scoreNasprotnika.text += pobraneKarteSkript[0].tockZmage.ToString() + "\n";
    }

    public GameObject Scoreboard;
    public GameObject MainUI;
    public TMP_Text tempWinTXT;
    protected void PokaziScore()
    {
        MainUI.SetActive(false);
        Scoreboard.SetActive(true);
        if (KdoJezmagalIgro() == lokalniIgralec.PobraneKarteSkript)
        {
            tempWinTXT.text = "you won";
            soundManager.Play("playerWon");
        } else
        {
            tempWinTXT.text = "you lost";
            soundManager.Play("playerLost");
        }
    }

    public void PreveriStKartIgralcev(int stKart)
    {
        podeliKarteIgralcu(stTrenutnegaIgralca, stKart - igralci[stTrenutnegaIgralca].karteIgralca.Count);
        if(stTrenutnegaIgralca==1)
            podeliKarteIgralcu(0, stKart - igralci[0].karteIgralca.Count);
        else
            podeliKarteIgralcu(1, stKart - igralci[1].karteIgralca.Count);

        /*for(int i = 0; i < stIgralcev; i++)
        {
            if (igralci[i].karteIgralca.Count < stKart)
            {
                podeliKarteIgralcu(i, stKart - igralci[i].karteIgralca.Count);
            }
        }*/
    }

    /*public void premakniKarto()
    {
        if (premaknjenaKarta != null)
        {
            if(premaknjenaKarta.lastnikKarte==igralci[stTrenutnegaIgralca].IDIgralca && lokalniIgralec == igralci[stTrenutnegaIgralca])
            {
                //CardScript.
            }
        }
    }*/

    public void IzbrisiVseKarte()
    {
        StopAllCoroutines();
        GameObject[] vseKarte = GameObject.FindGameObjectsWithTag("karta");

        for(int i =0; i < stIgralcev; i++)
        {
            igralci[i].jePripravljen = false;
            igralci[i].resetirajPodatkeIgralca();
        }

        deck.karte.Clear();

        for (int i = 0; i < vseKarte.Length; i++)
        {
            Destroy(vseKarte[i]);
        }
    }

    public void ZacniNovoIgro()
    {
        Start();
        MainUI.SetActive(true);
        Scoreboard.SetActive(false);
        posodobiTextTrenutnegaIgralca();
    }

    protected void dodajTockeZmagIgr()
    {
        PobraneKarteIgralca zmag = KdoJezmagalIgro();
        PobraneKarteIgralca zgub = VrniDrugePobraneKI(zmag);
        switch (stIgralcev)
        {
            case 2:
                {
                    if(tipIgre == TipIgre.navaden)
                    {
                        if (zgub.tocke == 0)
                            zmag.tockZmage += 3;
                        else if (zgub.tocke <= 32)
                            zmag.tockZmage += 2;
                        else
                            zmag.tockZmage += 1;
                        return;
                    } else if(tipIgre == TipIgre.zaprtAdut)
                    {
                        if (igralci[stIgralcaKiSpila].PobraneKarteSkript == zmag)
                        {
                            if (zgub.tocke == 0)
                                zmag.tockZmage += 3;
                            else if (zgub.tocke <= 32)
                                zmag.tockZmage += 2;
                            else
                                zmag.tockZmage += 1;
                            return;
                        }
                        else
                            zmag.tockZmage += 3;
                        return;
                    }
                    break;
                }
        }
    }

    public void posodobiInformacijeIgralcev()
    {
        //Debug.Log("Farbanje: " + farbanje);
        //lokalniIgralec.razvrstiRoko();
        for (int i = 0; i < stIgralcev; i++)
        {
            igralci[i].razvrstiRoko();
            igralci[i].simbolAduta = adut;
        }
        //IgralnoPolje.SimbolAduta = adut;

        //Tu dovolimo igralcu na vrsti premik kart, ali re�e botu naj naredi potezo
        if (lokalniIgralec == igralci[stTrenutnegaIgralca])
        { //Nastavimo da lahko samo lokalni igralec premika karte
            if(farbanje && IgralnoPolje.Karte.Count!=0)
                igralci[stTrenutnegaIgralca].lahkoPremikaKarte(true, IgralnoPolje.SimbolPrveKarte);
            else
                igralci[stTrenutnegaIgralca].lahkoPremikaKarte(true, "vse");
        }
        if (lokalniIgralec != igralci[stTrenutnegaIgralca])
        {
            lokalniIgralec.lahkoPremikaKarte(false, "prazno");
        }
        skrijAliPokaziKarte();
        posodobiTextTrenutnegaIgralca();
    }

    public void naslednjiIgralec()
    {
        if(stTrenutnegaIgralca==-1) //�e trenutni igralec �e ni nastavljen, ga naklju�no izberemo
            stTrenutnegaIgralca = UnityEngine.Random.Range(0, stIgralcev);
        else {
            stTrenutnegaIgralca++; //Izberemo naslednjega igralca
            if (stTrenutnegaIgralca == stIgralcev) //�e je �t naslednjega igralca ve�je od �t vseh igralcev, resetiramo indeks na 0
                stTrenutnegaIgralca = 0;
        }
        posodobiInformacijeIgralcev();
        //posodobiTextTrenutnegaIgralca();
        Debug.Log("Trenutni igralec " + stTrenutnegaIgralca);
        //igralci[stTrenutnegaIgralca].lahkoPremikaKarte();
    }

    public virtual void PremakniKartoVIgralnoPolje(GameObject karta)
    {
        bool doDelay = false;
        IgralnoPolje.SprejemiNovoKarto(karta);
        igralci[stTrenutnegaIgralca].OdstraniKarto(karta);

        if (igralci[stTrenutnegaIgralca] != lokalniIgralec && StKartNaPolju()==stIgralcev)
            doDelay = true;
        if (stIgralcev == IgralnoPolje.Karte.Count)
            stanjeIgre = StanjeIgre.rundaKoncala;
        else {
            naslednjiIgralec();
            stanjeIgre = StanjeIgre.rundaPoteka;
        }
        //Task.Delay(1).ContinueWith(t => predvajajIgro());
        //Invoke("predvajajIgro", 2f);
        //if (IgralnoPolje.Karte.Count == stIgralcev)
        if (doDelay)
            Invoke("predvajajIgro", 1.2f);
        else
            predvajajIgro();
    }

    public virtual bool ZamenjajAduta(GameObject noviAdut)
    {
        if (stIgralcev == 2)
        {
            CardScript ks = pridobiScriptKarte(noviAdut);
            if (ks.simbol == adut && ks.ime == "jack" && IgralnoPolje.Karte.Count==0) {
                if (ks.lastnikKarte == igralci[stTrenutnegaIgralca].IDIgralca)
                {
                    igralci[stTrenutnegaIgralca].OdstraniKarto(noviAdut);
                    GameObject stariAdut = deck.ZamenjajAduta(noviAdut);
                    ks.lahkoPremikas = false;
                    igralci[stTrenutnegaIgralca].sprejemiKarto(stariAdut);
                    igralci[stTrenutnegaIgralca].razvrstiRoko();
                    posodobiInformacijeIgralcev();
                    return true;
                }
            }
        }
        posodobiInformacijeIgralcev();
        return false;
    }

    public CardScript pridobiScriptKarte(GameObject kartaGO)
    {
        return (CardScript)kartaGO.GetComponent(typeof(CardScript));
    }

    public GameObject Preveri2040(GameObject karta)
    {
        if (IgralnoPolje.Karte.Count == 0)
        {
            CardScript sk = pridobiScriptKarte(karta);
            for(int i=0; i < igralci.Length; i++)
            {
                if (igralci[i].IDIgralca == sk.lastnikKarte)
                {
                    string iskan;
                    if (sk.ime == "queen")
                        iskan = "king";
                    else
                        iskan = "queen";
                    CardScript[] karteIgralca = igralci[i].pridobiScriptVsehKart();
                    for(int j=0; j < karteIgralca.Length; j++)
                    {
                        //Debug.Log("Simbol: " + karteIgralca[j].simbol + "\nIme: " + karteIgralca[j].ime + "\nIskan: " + iskan);
                        if (karteIgralca[j].simbol == sk.simbol && karteIgralca[j].ime == iskan)
                            return igralci[i].karteIgralca[j];
                    }
                }
            }
        }
        return null;
    }

    public virtual void IgralcuDodaj2040(GameObject prvaKarta,GameObject drugaKarta)
    {
        CardScript pk = pridobiScriptKarte(prvaKarta);
        CardScript dk = pridobiScriptKarte(drugaKarta);

        for(int i = 0; i < stIgralcev; i++)
        {
            if (igralci[i].IDIgralca == pk.lastnikKarte)
            {
                if (pk.simbol == adut)
                {
                    igralci[i].PobraneKarteSkript.tocke += 40;
                    soundManager.Play("met40");
                }
                else
                {
                    igralci[i].PobraneKarteSkript.tocke += 20;
                    soundManager.Play("met20");
                }
                    
                posodobiTextTrenutnegaIgralca();
                if (igralci[stTrenutnegaIgralca] != lokalniIgralec)
                {
                    dk.jeObrnjeno = false;
                    dk.obrniKarto();
                    StartCoroutine(PremakniDrugoKartoKPrvi(pk, dk));
                    //dk.premakniKarto(prvaKarta.transform.position - new Vector3(-2f, -2f, -1f), prvaKarta.transform.rotation);
                }
                /*if (PreveriCeJeKonecIgre())
                {
                    stanjeIgre = StanjeIgre.igraKoncala;
                    int temp = stTrenutnegaIgralca;
                }*/

                PremakniKartoVIgralnoPolje(prvaKarta);

                return;
            }
        }
    }

    private IEnumerator PremakniDrugoKartoKPrvi(CardScript pk, CardScript dk)
    {
        float elapsedTime=0f;
        while (elapsedTime < 1f)
        {
            dk.jeObrnjeno = false;
            dk.obrniKarto();
            dk.animFinished = true;
            dk.transform.SetPositionAndRotation(pk.transform.position - new Vector3(-2f, -2f, -1f), pk.transform.rotation);
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        yield return null;
    }

    protected bool preveriCeSoVseTockeDosezene()
    {
        for(int i = 0; i<stIgralcev; i++)
        {
            Debug.Log(igralci[i].PobraneKarteSkript.tockZmage + " i: " +i);
            if (igralci[i].PobraneKarteSkript.tockZmage >= stTockDoKonca)
                return true;
        }
        Debug.Log("Konec igre ni dose�ena");
        return false;
    }

    public CardScript NajvecjaKartaNaPoolju()
    {
        return IgralnoPolje.NajKarta();
    }
    [SerializeField]
    protected SoundManager soundManager;

    // Start is called before the first frame update
    private void Start()
    {
        
        Debug.Log("SP Start method");
        //soundManager = FindObjectOfType<SoundManager>();
        stanjeIgre = StanjeIgre.igraZacela;
        tipIgre = TipIgre.navaden;
        predvajajIgro();
    }

    public virtual void zapriDeck()
    {
        farbanje = true;
        dovoliInput();
        CardScript ks = pridobiScriptKarte(deck.karte[0]);
        ks.jeObrnjeno = true;
        //ks.m_Animator.SetTrigger("OnCloseDeck");

        tipIgre = TipIgre.zaprtAdut;
        stIgralcaKiSpila = stTrenutnegaIgralca;
    }

    public bool jeTrenutniIgralecLokalni()
    {
        if (lokalniIgralec == igralci[stTrenutnegaIgralca])
            return true;
        return false;
    }

    public void blokirajInput()
    {
        lokalniIgralec.lahkoPremikaKarte(false, "");
    }
    public void dovoliInput()
    {
        if (lokalniIgralec == igralci[stTrenutnegaIgralca])
            lokalniIgralec.lahkoPremikaKarte(true, "vse");
    }

    // Update is called once per frame
    /*void Update()
    {
        //skrijAliPokaziKarte();
        //lokalniIgralec.razvrstiRoko();
        //nakljucnoIzberiIgralca();
        //premakniKarto();
    }*/
    /*****************************************
    ******************* UI *******************
    *****************************************/
    public void zapustiIgro()
    {
        SceneManager.LoadScene("mainMenu", LoadSceneMode.Single);
    }
    public void nastaviSimbolAduta()
    {
        if (atlasSimbolov == null)
            atlasSimbolov = Resources.Load<SpriteAtlas>("CardsSpriteAtlas/Simboli");
        UI_SlikaAduta.sprite = atlasSimbolov.GetSprite((adut + "Adut"));
    }
    public void posodobiTextTrenutnegaIgralca()
    {
        if(lokalniIgralec==igralci[stTrenutnegaIgralca])
            UI_trenutniIgralec.text = "Current player: YOU";
        else
            UI_trenutniIgralec.text = "Current player: " + igralci[stTrenutnegaIgralca].imeIgralca;
        UI_tockeIgralca.text = lokalniIgralec.PobraneKarteSkript.tocke.ToString();
    }
}
