using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public GameObject kartaPrefab;
    [SerializeField]
    public List<GameObject> karte = new List<GameObject>();
    public GameObject adut = null;
    public GameManagerScript GameManager;
    public GameObject ConfirmPupUP;

    public void ustvariDeck()
    {
        Debug.Log("Ustvarjam karte");
        string[] simboli = { "clubs", "diamonds", "spades", "hearts" };
        string[] ime = { "ace", "10", "king", "queen", "jack" };
        short[] vrednosti = { 11, 10, 4, 3, 2 };
        karte = new List<GameObject>();
        Vector3 lokacijaNoveKarte = new Vector3(0, 0, 0);
        for (int i = 0; i < 4; i++) //Ustvarimo 20 kart
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject go = Instantiate(kartaPrefab, lokacijaNoveKarte, Quaternion.identity) as GameObject; //ustvarimo gameobject iz prefaba karte in ga shranimo v variable go
                CardScript drugiScript = (CardScript)go.GetComponent(typeof(CardScript)); //pridobimo skript te karte
                drugiScript.nastaviLastnosti(simboli[i], ime[j], vrednosti[j], true, GameManager); //poklièemo funkcijo in karti nastavimo vrednost
                drugiScript.nastaviSprite(); //nastavimo sliko karte
                drugiScript.obrniKarto(); //pogledamo èe mora biti obrnjena
                go.transform.parent = GameObject.Find("Deck").transform; //nastavimo, da je parentano na deck
                karte.Add(go); //dodamo karto na koncu lista
            }
        }
        premesajKarte(); //Premesano karte, da niso po vrsti
        premesajKarte();
        razvrstiKarte(); //postavimo karte tako, da zgledajo koda so razvršèene v decku
    }

    public void premesajKarte()
    {
        GameObject tempGO;
        for (int i = 0; i < karte.Count; i++)
        {
            int rnd = Random.Range(0, karte.Count);
            tempGO = karte[rnd];
            karte[rnd] = karte[i];
            karte[i] = tempGO;
        }
    }

    void razvrstiKarte()
    {
        Vector3 lokacijaKarte = transform.position;
        lokacijaKarte.z += 2f;
        for (int i = 0; i < karte.Count; i++)
        {
            CardScript drugiScript = (CardScript)karte[i].GetComponent(typeof(CardScript));
            drugiScript.premakniKarto(lokacijaKarte, new Quaternion(0, 0, 0, 1));
            lokacijaKarte.x += 0.01f;
            lokacijaKarte.y += 0.01f;
            lokacijaKarte.z -= 0.01f;
        }
    }

    public CardScript pridobiScriptKarte(GameObject kartaGO)
    {
        return (CardScript)kartaGO.GetComponent(typeof(CardScript));
    }

    public GameObject ZamenjajAduta(GameObject noviAdut)
    {
        if (karte.Count != 0){
            CardScript noviKs = pridobiScriptKarte(noviAdut);

            Vector3 novaPozicija = transform.position;
            novaPozicija.x -= 2.5f;
            novaPozicija.z = 5f;
            noviKs.premakniKarto(novaPozicija, new Quaternion(0, 0, 1, 1));

            //noviKs.premakniKarto(karte[0].transform.position, karte[0].transform.rotation);
            noviKs.jeObrnjeno = false;
            noviKs.obrniKarto();

            GameObject tempKard = karte[0];
            karte.RemoveAt(0);
            karte.Insert(0, noviAdut);

            return tempKard;
        }
        return null;
    }

    public string ustvariAduta()
    {
        Debug.Log("Ustvarjam ADUTA");
        adut = karte.Last();
        karte.RemoveAt(karte.Count - 1);
        karte.Insert(0, adut);
        CardScript skriptKarte = pridobiScriptKarte(adut);
        skriptKarte.jeObrnjeno = false;
        skriptKarte.obrniKarto();
        Vector3 novaPozicija= transform.position;
        novaPozicija.x -= 2.5f;
        novaPozicija.z = 5f;
        skriptKarte.premakniKarto(novaPozicija, new Quaternion(0, 0, 1, 1));
        return skriptKarte.simbol;
    }
    public void podeliKartoIgralcu(PlayerScript igralec)
    {
        karte.Last().transform.parent = igralec.transform;
        igralec.sprejemiKarto(karte.Last());
        CardScript skriptKarte = pridobiScriptKarte(karte.Last());
        skriptKarte.lastnikKarte = igralec.IDIgralca;
        karte.RemoveAt(karte.Count - 1);
        igralec.razvrstiRoko();
    }
    public void podeliKarteIgralcu(PlayerScript igralec, int stKart)
    {
        CardScript skriptKarte;
        for (int i = 0; i < stKart; i++) { 
            karte.Last().transform.parent = igralec.transform;
            igralec.sprejemiKarto(karte.Last());
            skriptKarte = pridobiScriptKarte(karte.Last());
            skriptKarte.lastnikKarte = igralec.IDIgralca;
            karte.RemoveAt(karte.Count - 1);
        }
        igralec.razvrstiRoko();
    }

    private void OnMouseUp()
    {
        if(GameManager.stIgralcev==2 && GameManager.IgralnoPolje.Karte.Count==0 && GameManager.jeTrenutniIgralecLokalni() && karte.Count>1 && GameManager.farbanje!=true)
        {
            GameManager.blokirajInput();
            ConfirmPupUP.SetActive(true);
        }
    }
}
