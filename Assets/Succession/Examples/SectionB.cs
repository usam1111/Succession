using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Itach.Succession;

public class SectionB : SectionBase
{
    [SerializeField] private Text text;
    [SerializeField] private Image panel;
    private Color panelColor;

    // 初期設定
    public override SectionNode Initialize(string id)
    {
        panel.gameObject.SetActive(false);
        panelColor = panel.color;
        return base.CreateSectionNode(id, this);
    }
    //-------------------------------------------------------------------------
    // ロード処理
    public override IEnumerator AtSectionLoad()
    {
        panel.gameObject.SetActive(true);
        panel.color = panelColor;
        text.text = "";
        yield return new WaitForSeconds(0.2f);
        text.text = id;
        yield return new WaitForSeconds(0.2f);
        yield break;
    }
    //-------------------------------------------------------------------------
    // 到着処理
    public override IEnumerator AtSectionInit()
    {
        text.text = id + " : --";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : A---";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Ac----";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Act---";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Acti--";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Activ-";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Active";
        yield break;
    }
    //-------------------------------------------------------------------------
    // 出発処理
    public override IEnumerator AtSectionGoto()
    {
        text.text = id + " : Activ-";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Acti--";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Act---";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : Ac--";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : A-";
        yield return new WaitForSeconds(0.05f);
        text.text = id + " : -";
        yield return new WaitForSeconds(0.05f);
        text.text = id + "";
        yield return new WaitForSeconds(0.05f);
        yield break;
    }
    //-------------------------------------------------------------------------
    // アンロード処理
    public override IEnumerator AtSectionUnload()
    {
        text.text = "";
        yield return new WaitForSeconds(0.2f);
        panel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0.5f);
        yield return new WaitForSeconds(0.2f);
        panel.gameObject.SetActive(false);
        yield break;
    }
    //-------------------------------------------------------------------------
}
