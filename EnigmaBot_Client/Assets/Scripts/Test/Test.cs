using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Test : MonoBehaviour
{
    //public Text text;
    public Image image;
    

    private void OnEnable()
    {
        Debug.Log("Enable");
    }
    void Start()
    {
        Debug.Log("Start");
        image.DOFade(1f, 3f).OnComplete(() => { image.DOFade(0f, 6f); }); 
        //text.DOText("This text will replace the existing one", 3).SetEase(Ease.Linear);

        //image.DOColor(RandomColor(), 1.5f).SetEase(Ease.Linear);

        //image.DOFillAmount(0, 1.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).OnStepComplete(() => {
        //    image.fillClockwise = !image.fillClockwise;
        //    image.DOColor(RandomColor(), 1.5f).SetEase(Ease.Linear);
        //});

        //image.DOFade(0, 1.5f).SetLoops(-1,LoopType.Restart);
        
        //Koreographer.Instance.RegisterForEvents(eventID, FireEventDebugLog);
    }

    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject bullet =  ObjectPoolCtrl.Instance.Spawn("Bullet") as GameObject;
            bullet.transform.position = Vector3.zero;
            bullet.transform.rotation = Quaternion.identity;
            bullet.transform.SetParent(gameObject.transform);
            bullet.GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Debug.Log("qqqqqqqqqqqqqqqqqq");
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("ddddddddddddddddddddd");
            }
        }
    }

   // public string eventID;//事件ID，之前设置的那个ID

    //void FireEventDebugLog(KoreographyEvent koreoEvent)
    //{
    //    GameObject go = Resources.Load<GameObject>("Cube");
    //    Instantiate(go);
    //}

}
