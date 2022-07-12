using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum tipIgralca { igralec, nasprotnik, prijatelj }

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> karteIgralca = new List<GameObject>();
    [Header("Informaijce Igralca")]
    public string imeIgralca;
    [SerializeField]
    public string IDIgralca;
    public bool jeBot;
    public short stIgralca;
    public bool jePripravljen;
    [Header("Roka igralca")]
    public string simbolAduta;
    public short razdalja = 20;
    //public Vector3 pozicija;
    public PobraneKarteIgralca PobraneKarteSkript;


    public void resetirajPodatkeIgralca()
    {
        karteIgralca.Clear();
        PobraneKarteSkript.Karte.Clear();
        PobraneKarteSkript.tocke = 0;
    }

    public void sprejemiKarto(GameObject karta)
    {
        karteIgralca.Add(karta);
        CardScript ks = pridobiScriptKarte(karta);
        ks.lastnikKarte = IDIgralca;
        //razvrstiRoko();
    }

    public void OdstraniKarto(GameObject karta)
    {
        for(int i=0;i< karteIgralca.Count; i++)
        {
            if(karta == karteIgralca[i])
            {
                karteIgralca.RemoveAt(i);
                return;
            }
        }
        Debug.Log("ERROR: Karta ne obstaja na itbranem igralcu");
    }

    public CardScript pridobiScriptKarte(GameObject kartaGO)
    {
        return (CardScript)kartaGO.GetComponent(typeof(CardScript));
    }
    public CardScript[] pridobiScriptVsehKart()
    {
        CardScript[] vseKarte = new CardScript[karteIgralca.Count];
        for(int i=0; i < karteIgralca.Count; i++)
        {
            vseKarte[i] = pridobiScriptKarte(karteIgralca[i]);
        }
        return vseKarte;
    }

    public void razvrstiListKart()
    {
        if(karteIgralca.Count!=0)
        {
            List<GameObject> aduti = new List<GameObject>();
            List<GameObject> hearts = new List<GameObject>();
            List<GameObject> spades = new List<GameObject>();
            List<GameObject> diamonds = new List<GameObject>();
            List<GameObject> clubs = new List<GameObject>();
            var vsotaPosSimbolov = new Dictionary<string, int>
            {
                { "hearts", 0 },
                { "spades", 0 },
                { "clubs", 0 },
                { "diamonds", 0 }
            };
            CardScript karta;
            for(int i=0; i < karteIgralca.Count; i++)
            {
                karta = pridobiScriptKarte(karteIgralca[i]);
                if (karta.simbol == simbolAduta)
                    aduti.Add(karteIgralca[i]);
                else
                {
                    switch (karta.simbol)
                    {
                        case "hearts":
                            hearts.Add(karteIgralca[i]);
                            vsotaPosSimbolov["hearts"] += karta.vrednost;
                            break;
                        case "spades":
                            spades.Add(karteIgralca[i]);
                            vsotaPosSimbolov["spades"] += karta.vrednost;
                            break;
                        case "clubs":
                            clubs.Add(karteIgralca[i]);
                            vsotaPosSimbolov["clubs"] += karta.vrednost;
                            break;
                        case "diamonds":
                            diamonds.Add(karteIgralca[i]);
                            vsotaPosSimbolov["diamonds"] += karta.vrednost;
                            break;
                    }
                }

                    //vsotaPosSimbolov[karta.simbol] += karta.vrednost;
            }

            razvrstiListKartPoVrednosti(aduti);
            karteIgralca = aduti;

            for (int i = 0; i < 4; i++)
            {
                if (vsotaPosSimbolov.Values.Max() != 0)
                {
                    switch (vsotaPosSimbolov.Aggregate((l, r) => l.Value > r.Value ? l : r).Key)
                    {
                        case "hearts":
                            razvrstiListKartPoVrednosti(hearts);
                            karteIgralca.AddRange(hearts);
                            vsotaPosSimbolov["hearts"] = 0;
                            break;
                        case "spades":
                            razvrstiListKartPoVrednosti(spades);
                            karteIgralca.AddRange(spades);
                            vsotaPosSimbolov["spades"] = 0;
                            break;
                        case "clubs":
                            razvrstiListKartPoVrednosti(clubs);
                            karteIgralca.AddRange(clubs);
                            vsotaPosSimbolov["clubs"] = 0;
                            break;
                        case "diamonds":
                            razvrstiListKartPoVrednosti(diamonds);
                            karteIgralca.AddRange(diamonds);
                            vsotaPosSimbolov["diamonds"] = 0;
                            break;
                    }
                }
                else { 
                    //Debug.Log("Prazno");
                    break;
                }
            }
            //Debug.Log(vsotaPosSimbolov.Aggregate((l, r) => l.Value > r.Value ? l : r).Key);
            //Debug.Log(vsotaPosSimbolov.Values.Max());
            //Debug.Log(vsotaPosSimbolov["hearts"]);
        }
    }
    private void razvrstiListKartPoVrednosti(List<GameObject> karte)
    {
        
        CardScript karta;
        CardScript karta2;
        GameObject tmp;
        //Debug.Log("Št kart"+karte.Count);
        for (int i = 0; i < karte.Count; i++)
        {
            for(int j = i + 1; j < karte.Count; j++)
            {
                karta = pridobiScriptKarte(karte[i]);
                karta2 = pridobiScriptKarte(karte[j]);
                if ((karta2.vrednost < karta.vrednost ) /*&& karta2.simbol == simbolAduta && karta.simbol == simbolAduta*/)
                {
                    tmp = karte[i];
                    karte[i] = karte[j];
                    karte[j] = tmp;
                    //Debug.Log("i "+i+"\nj "+j);
                }/* else if(karta2.simbol!=karta.simbol)
                {
                    tmp = karte[i];
                    karte[i] = karte[j];
                    karte[j] = tmp;
                }*/
            }
        }
    }

    public void razvrstiRoko()
    {
        razvrstiListKart();
        /*float razX;
        razX = (float)razdalja / (karteIgralca.Count + 1);
        //Vector3 lokacijaKarte = new Vector3(-(razdalja/2), -10, 0);
        Vector3 lokacijaKarte = transform.position;
        if (stIgralca > 1)
        {
            lokacijaKarte.y -= razdalja / 2;
        }
        else
        {
            lokacijaKarte.x -= razdalja / 2;
        }
        for (int i = 0; i < karteIgralca.Count; i++)
        {
            if (stIgralca > 1)
            {
                lokacijaKarte.y += razX;
            } else { 
                lokacijaKarte.x += razX;
            }
            lokacijaKarte.z -= 0.01f;
            CardScript drugiScript = (CardScript)karteIgralca[i].GetComponent(typeof(CardScript));
            drugiScript.premakniKarto(lokacijaKarte, transform.rotation);
        }*/
        float razX;
        razX = 5.2f; //razdalja;
        //Vector3 lokacijaKarte = new Vector3(-(razdalja/2), -10, 0);
        Vector3 lokacijaKarte = transform.position;
        lokacijaKarte.z = 0;
        if (stIgralca > 1)
        {
            lokacijaKarte.y -= razdalja;
        }
        else
        {
            lokacijaKarte.x -= ((karteIgralca.Count + 1) * razX) / 2;
        }
        CardScript drugiScript;
        for (int i = 0; i < karteIgralca.Count; i++)
        {
            if (stIgralca > 1)
            {
                lokacijaKarte.y += razX;
            }
            else
            {
                lokacijaKarte.x += razX;
            }
            drugiScript = (CardScript)karteIgralca[i].GetComponent(typeof(CardScript));
            drugiScript.premakniKarto(lokacijaKarte, transform.rotation);
        }
    }

    public void skrijKarte() //Obrne karto;
    {
        for (short i = 0; i < karteIgralca.Count; i++)
        {
            CardScript drugiScript = (CardScript)karteIgralca[i].GetComponent(typeof(CardScript));
            drugiScript.jeObrnjeno = true;
            drugiScript.obrniKarto();
        }
    }
    public void pokaziKarte() //Obrne karte tako, da vidimo njihovo vrednost
    {
        //Debug.Log("deluje");
        for (short i = 0; i < karteIgralca.Count; i++)
        {
            CardScript drugiScript = (CardScript)karteIgralca[i].GetComponent(typeof(CardScript));
            drugiScript.jeObrnjeno = false;
            drugiScript.obrniKarto();
            //drugiScript.lahkoPremikas = true;
        }
    }

    public void lahkoPremikaKarte(bool odl, string DovSimbol)
    {
        //Debug.Log(simbolAduta);
        CardScript sk;
        if (odl == false) { 
            for (short i = 0; i < karteIgralca.Count; i++)
            {
                sk = pridobiScriptKarte(karteIgralca[i]);
                sk.lahkoPremikas = false;
            }
        } else
        {
            int pozKart = 0;
            for (short i = 0; i < karteIgralca.Count; i++)
            {
                sk = pridobiScriptKarte(karteIgralca[i]);
                if(sk.simbol == DovSimbol || DovSimbol == "vse") { 
                    sk.lahkoPremikas = true;
                    pozKart++;
                } else
                    sk.lahkoPremikas = false;
            }
            if (pozKart == 0 && karteIgralca.Count != 0)
            {
                for (short i = 0; i < karteIgralca.Count; i++)
                {
                    sk = pridobiScriptKarte(karteIgralca[i]);
                    if (sk.simbol == simbolAduta) { 
                        sk.lahkoPremikas = true;
                        pozKart++;
                    } else
                        sk.lahkoPremikas = false;
                } 
            }
            if (pozKart == 0 && karteIgralca.Count!=0)
                lahkoPremikaKarte(true, "vse");
        }
    }

    public GameObject BotNarediPotezo(List<GameObject> karteNaPolju, int stIgralcev, CardScript najKarta, bool farbanje)
    {
        Debug.Log("Izbiram karto");
        List<CardScript> ks = new List<CardScript>();
        for(int i = 0; i < karteIgralca.Count; i++)
        {
            ks.Add(pridobiScriptKarte(karteIgralca[i]));
        }
        if (karteNaPolju.Count == 0) //Preverimo, èe je prvi na vrsti
        {
            for(int i = 0; i < ks.Count; i++)
            {
                if (ks[i].simbol == simbolAduta && ks[i].ime == "jack" && ks[i].StKartNaPolju()==0 && ks[i].StKartVDecku()!=0)
                {
                    Debug.Log("menjam aduta");
                    ks[i].CallZamenjajAduta();
                }
            }
            ks.Clear();
            for (int i = 0; i < karteIgralca.Count; i++)
            {
                ks.Add(pridobiScriptKarte(karteIgralca[i]));
            }
            GameObject temp;
            for (int i = 0; i < ks.Count; i++) // preverimo ce ima igralec 20 ali 40
            {
                Debug.Log("Preverjam 20 40");
                if (ks[i].ime == "king" || ks[i].ime == "queen")
                {
                    Debug.Log("Nasel mozno 20 40");
                    temp = ks[i].callPreveri2040();
                    if (temp != null)
                    {
                        Debug.Log("Ven mecem 20/40");
                        ks[i].premakniV2040(temp);
                        return null;
                    }
                }
            }
            int najmanjsaKarta = ks.Count-1;
            for (int i = 0; i < ks.Count; i++)
            {
                if (ks[najmanjsaKarta].vrednost >= ks[i].vrednost && ks[i].simbol!= simbolAduta)
                    najmanjsaKarta = i;
            }
            return karteIgralca[najmanjsaKarta];
        } else {
            if (farbanje)
            {//Farbanje je potrebno in na polju so karte
                int indKarte = 0, stAdutov=0, stKart=0;
                for(int i = 0; i < ks.Count; i++)
                {
                    if(ks[i].simbol==simbolAduta)
                    {
                        stAdutov++;
                        if(ks[indKarte].vrednost>ks[i].vrednost)
                            indKarte = i;
                    }
                    if (ks[i].simbol==najKarta.simbol)
                    {
                        stKart++;
                        indKarte = i;                  
                    }
                }
                if (stKart != 0 || stAdutov!=0)
                    return karteIgralca[indKarte];
                else
                {
                    int najmanjsaKarta = ks.Count - 1;
                    for (int i = 0; i < ks.Count; i++)
                    {
                        if (ks[najmanjsaKarta].vrednost >= ks[i].vrednost && ks[i].simbol != simbolAduta)
                            najmanjsaKarta = i;
                    }
                    return karteIgralca[najmanjsaKarta];
                }
            } else
            { //Farbanje ni potrebno
                bool poberem = false;
                int indKarte = 0;
                for (int i = 0; i < ks.Count; i++)
                {
                    if (ks[i].simbol == najKarta.simbol)
                    {
                        if (ks[i].vrednost > najKarta.vrednost && najKarta.simbol!=simbolAduta)
                        {
                            indKarte = i;
                            poberem = true;
                        }
                    }
                }
                if (poberem)
                    return karteIgralca[indKarte];

                for (int i = 0; i < ks.Count; i++) { 
                    if (ks[i].simbol == simbolAduta) {
                        if (najKarta.vrednost >= 10 && najKarta.simbol != simbolAduta)
                        {
                            indKarte = i;
                            poberem = true;
                            break;
                        }
                    }
                }

                if(poberem)
                    return karteIgralca[indKarte];
                else
                {
                    int najmanjsaKarta = ks.Count - 1;
                    for (int i = 0; i < ks.Count; i++)
                    {
                        if (ks[najmanjsaKarta].vrednost >= ks[i].vrednost && ks[i].simbol != simbolAduta)
                            najmanjsaKarta = i;
                    }
                    return karteIgralca[najmanjsaKarta];
                }
            }
        }
        //return karteIgralca[Random.RandomRange(0, karteIgralca.Count)];
    }
}
