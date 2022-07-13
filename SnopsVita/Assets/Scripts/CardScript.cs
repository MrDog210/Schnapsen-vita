using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
public class CardScript : MonoBehaviour
{
    [SerializeField]
    public string lastnikKarte;
    [Header("Informacije karte")]
    [SerializeField]
    public string simbol;
    [SerializeField]
    public string ime;
    [SerializeField]
    public short vrednost;
    [SerializeField]
    public bool jeObrnjeno;
    public bool lahkoPremikas;
    private bool tempJeObrnjeno;
    [Header("Povezave na objekte")]
    public SpriteAtlas atlasKart;
    //public Animator m_Animator;
    private SpriteRenderer CardSprite;
    private Transform cardTransform;
    private Vector2 staraPozicija;
    private Vector2 razlikaPozicije;
    public GameManagerScript GameManager;
    private GameObject drugaKarta;

    /*public bool flip;
    private Animator m_animator;*/
    public void nastaviLastnosti(string s, string i, short v, bool obr, GameManagerScript gm){
        simbol=s;
        ime=i;
        vrednost=v;
        jeObrnjeno=obr;
        tempJeObrnjeno = obr;
        //BoxCollider2D bc = gameObject.AddComponent<BoxCollider2D>();
        //gameObject.GetComponent<BoxCollider2D>();
        lahkoPremikas = false;
        GameManager = gm;
        drugaKarta = null;
    }
    private void Awake()
    {

        atlasKart = Resources.Load<SpriteAtlas>("CardsSpriteAtlas/" + PlayerPrefs.GetString("izbranSkinKart") + "/MasterAtlas");
    }
    // Start is called before the first frame update
    void Start()
    {
        CardSprite = gameObject.GetComponent<SpriteRenderer>();
        cardTransform = gameObject.GetComponent<Transform>();
    }
    public void nastaviSprite(){
        CardSprite = gameObject.GetComponent<SpriteRenderer>();
        SpremeniFaceKarte();
        //CardSprite.sprite= Resources.Load<Sprite>("Cards/"+ PlayerPrefs.GetString("izbranSkinKart") + "/"+ime+"_of_"+simbol);
    }
    
    public void obrniKarto()
    {
        if (tempJeObrnjeno != jeObrnjeno) {
            tempJeObrnjeno = jeObrnjeno;
            //m_Animator.SetTrigger("NaObrniKarto");
            SpremeniFaceKarte();
        }
    }
    public void SpremeniFaceKarte()
    {
        if (jeObrnjeno)
        {
            CardSprite.sprite = atlasKart.GetSprite("cardBack"); // Resources.Load<Sprite>("Cards/" + PlayerPrefs.GetString("izbranSkinKart") + "/cardBack");
        }
        else
        {
            CardSprite.sprite = atlasKart.GetSprite((ime + "_of_" + simbol)); //Resources.Load<Sprite>("Cards/" + PlayerPrefs.GetString("izbranSkinKart") + "/" + ime + "_of_" + simbol);
        }
    }

    public float animTime = 10;
    public Vector3 noviKord;
    public Quaternion noviRotation;
    public void premakniKarto(Vector3 noviKord2, Quaternion rotation)
    {
        noviKord = noviKord2;
        noviRotation = rotation;
        gameObject.transform.rotation = rotation;
        //if(!animFinished)
        //    StopAllCoroutines();
        animFinished = false;
        //StartCoroutine(PredvajajAnimPremikKarte(noviKord2, rotation));
    }

    IEnumerator PredvajajAnimPremikKarte(Vector3 noviKord2, Quaternion rotation)
    {
        float razdalja;
        while (!animFinished)
        {
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, noviKord2, animTime * Time.deltaTime);
            //gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, noviRotation, 5 * Time.deltaTime);
            razdalja = Vector3.Distance(noviKord2, gameObject.transform.position);
            if (razdalja <= 0.01)
            {
                animFinished = true;
                gameObject.transform.position = noviKord2;
            }
            if (gameObject.transform.position == noviKord2)
                animFinished = true;
            yield return null;
        }
        yield return null;
    }
    public GameObject callPreveri2040()
    {
        return GameManager.Preveri2040(this.gameObject);
    }

    public void premakniV2040(GameObject dk)
    {
        GameManager.IgralcuDodaj2040(this.gameObject, dk);
    }

    void OnMouseDown()
    {
        if (!animFinished)
        {
            animFinished = true;
            staraPozicija = noviKord;
        } else
            staraPozicija = transform.position;

        if (lahkoPremikas && animFinished)
        {
            razlikaPozicije = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            razlikaPozicije -= staraPozicija;
            if(ime=="queen" || ime == "king")
            {
                Debug.Log("20/40");
                drugaKarta = GameManager.Preveri2040(this.gameObject);
                //Debug.Log(drugaKarta.transform.position);
                if (drugaKarta != null)
                    Debug.Log("Na�el!");
            }
        }
    }
    void OnMouseDrag()
    {
        if (lahkoPremikas && animFinished)
        {
            Vector2 pozicijaMiske = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = pozicijaMiske - razlikaPozicije;
            transform.localPosition -= new Vector3(0, 0, 2f);
            if (drugaKarta != null)
            {
                //Debug.Log(drugaKarta.transform);
                CardScript sk = (CardScript)drugaKarta.GetComponent(typeof(CardScript));
                sk.animFinished = true;
                drugaKarta.transform.position = gameObject.transform.position - new Vector3(2f,2f,-1f);
            }
            //Debug.Log(pozicijaMiske);
        }
    }

    public void OnMouseUp()
    {
        if (lahkoPremikas && animFinished) {
            RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100);
            //RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            for(int i = 0; i < hit.Length; i++)
            {
                //Debug.Log(hit[i].collider.gameObject.tag);
                if (hit[i].collider.gameObject.tag == "DropZone")
                {
                    if (drugaKarta != null) { 
                        GameManager.IgralcuDodaj2040(this.gameObject, drugaKarta);
                        return;
                    }
                    GameManager.PremakniKartoVIgralnoPolje(this.gameObject);
                    return;
                }
                else if (hit[i].collider.gameObject.tag == "Deck")
                {
                    Debug.Log("Menjava aduta");
                    GameManager.ZamenjajAduta(this.gameObject);
                    return;
                }
            }
            if (drugaKarta != null)
                GameManager.posodobiInformacijeIgralcev();
            premakniKarto(staraPozicija, gameObject.transform.rotation);
            //transform.position = staraPozicija;
            transform.position += new Vector3(0, 0, 0);
        }
    }

    public void CallZamenjajAduta()
    {
        GameManager.ZamenjajAduta(this.gameObject);
    }

    public int StKartNaPolju()
    {
        return GameManager.StKartNaPolju();
    }

    public int StKartVDecku()
    {
        return GameManager.StKartVDecku();
    }

    public bool animFinished=true;
    // Update is called once per frame
    void LateUpdate()
    {
        if (!animFinished) { 
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, noviKord, animTime * Time.deltaTime);
            //gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, noviRotation, animTime * Time.deltaTime);
            //Debug.Log(gameObject.transform.position.x);
            //Debug.Log(noviKord.x);
            //if (gameObject.transform.position.x >= noviKord.x + 0.01f)
            float razdalja = Vector3.Distance(noviKord, gameObject.transform.position);
            if (razdalja <= 0.01)
            {
                animFinished = true;
                gameObject.transform.position = noviKord;
            }
            if (gameObject.transform.position == noviKord)
                animFinished = true;
        }
        //obrniKarto();
    }
}
