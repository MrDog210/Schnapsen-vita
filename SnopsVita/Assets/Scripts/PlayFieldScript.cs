using System.Collections.Generic;
using UnityEngine;

public class PlayFieldScript : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> Karte;
    public string SimbolPrveKarte;
    public string SimbolAduta;
    public enum nacinZlaganja
    {
        navaden,
        mali
    };
    public nacinZlaganja NacinZlaganja;
    private void Awake()
    {
        NacinZlaganja = nacinZlaganja.navaden;
    }
    public void SprejemiNovoKarto(GameObject karta)
    {
        CardScript kartaScript = (CardScript)karta.GetComponent(typeof(CardScript));
        kartaScript.lahkoPremikas = false;
        kartaScript.jeObrnjeno = false;
        kartaScript.obrniKarto();
        Karte.Add(karta);
        RazvrstiKarte();
    }
    public CardScript pridobiScriptKarte(GameObject kartaGO)
    {
        return (CardScript)kartaGO.GetComponent(typeof(CardScript));
    }
    public void RazvrstiKarte()
    {
        switch (NacinZlaganja)
        {
            case nacinZlaganja.navaden:
            {
                if (Karte.Count == 1)
                {
                    CardScript kartaSkript = pridobiScriptKarte(Karte[0]);
                    kartaSkript.premakniKarto(new Vector3(0f, 1f, 0f),Quaternion.Euler(0, 0, Random.Range(-35, 35)));
                    SimbolPrveKarte = kartaSkript.simbol;
                } else
                {
                    Vector3 novaPoz = gameObject.transform.position; //Karte[0].transform.position;
                    novaPoz.z = Karte[Karte.Count - 2].transform.position.z;
                    novaPoz.z -= 0.1f;
                    CardScript kartaSkript = pridobiScriptKarte(Karte[Karte.Count - 1]);
                    kartaSkript.premakniKarto(novaPoz, Quaternion.Euler(0, 0, Random.Range(-35, 35)));
                }
                break;
            }
            case nacinZlaganja.mali:
            {
                break;
            }
        }
    }

    public string KateriIgralecPobere()
    {
        /*string idIgralca;
        CardScript sk = pridobiScriptKarte(Karte[0]);
        idIgralca = sk*/
        List<CardScript> sk = new List<CardScript>();
        int najKarta = 0;

        for (int i = 0; i < Karte.Count; i++)
        {
            sk.Add(pridobiScriptKarte(Karte[i]));
        }
        for (int i = 1; i < Karte.Count; i++)
        {
            if (sk[najKarta].simbol != SimbolAduta)
            {
                if (sk[i].simbol == SimbolAduta)
                    najKarta = i;
                else if (sk[najKarta].simbol == sk[i].simbol && sk[najKarta].vrednost < sk[i].vrednost)
                    najKarta = i;
            } else
            {
                if (sk[najKarta].simbol == sk[i].simbol && sk[najKarta].vrednost < sk[i].vrednost)
                    najKarta = i;
            }
        }
            
        return sk[najKarta].lastnikKarte;
    }

    public CardScript NajKarta()
    {
        /*string idIgralca;
        CardScript sk = pridobiScriptKarte(Karte[0]);
        idIgralca = sk*/
        if (Karte.Count == 0)
            return null;

        List<CardScript> sk = new List<CardScript>();
        int najKarta = 0;

        for (int i = 0; i < Karte.Count; i++)
        {
            sk.Add(pridobiScriptKarte(Karte[i]));
        }
        for (int i = 1; i < Karte.Count; i++)
        {
            if (sk[najKarta].simbol != SimbolAduta)
            {
                if (sk[i].simbol == SimbolAduta)
                    najKarta = i;
                else if (sk[najKarta].simbol == sk[i].simbol && sk[najKarta].vrednost < sk[i].vrednost)
                    najKarta = i;
            }
            else
            {
                if (sk[najKarta].simbol == sk[i].simbol && sk[najKarta].vrednost < sk[i].vrednost)
                    najKarta = i;
            }
        }

        return sk[najKarta];
    }

    public void IzprazniListKart()
    {
        Karte.Clear();
    }
}
