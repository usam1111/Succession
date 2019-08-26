# Succession

「Succession」はAS3のProgressionを参考にして作られた、Unity用の画面遷移フレームワークです。

![](https://img.shields.io/badge/Unity-2018.3-red.svg)
![](https://img.shields.io/badge/.NET-4.x-yellow.svg)
[![](https://img.shields.io/badge/License-MIT-green)](https://github.com/usam1111/Succession/blob/master/LICENSE)

## UnityPackage
UnityPackageは以下からダウンロードできます。
- [Succession_v1.0.0.unitypackage](https://github.com/usam1111/Succession/blob/master/Succession_v1.0.0.unitypackage)

## Progression と Succession の違い
- Progressionは汎用的で大規模なフレームワークですが、SuccessionはProgressionでいうSceneのような、画面遷移の機能だけに特化しています。
- Sceneという名称はUnityでは紛らわしいので、代わりにSectionという名称を使用しています。
- ProgressionでのCommandの代わりに、SuccessionではC#のコルーチンを使用しています。
- Successionでは、例えば複数の画面ごとに独立した画面遷移を行いたい場合、Successionクラスを複数インスタンス化することで、それぞれ個別に画面遷移させることができます。

## Succession の概要
- Successionクラスをインスタンス化し、そこに各セクションをツリー構造で登録していきます。
- ツリー構造にすることで、例えば「/a/b/c」へ移動するといった、パスを使った画面遷移が行えるようになります。この場合、aとbとcはそれぞれ1つのセクションになり、cの親階層にbがあり、bの親階層にaがあります。
- 各セクションは以下の4つのコルーチンを持っていて、移動の進行に合わせて順次実行されます。
  - AtSectionLoad : ロード処理
  - AtSectionInit : 開始処理（到着地点のセクションでのみ実行される）
  - AtSectionGoto : 終了処理（出発地点のセクションでのみ実行される）
  - AtSectionUnload : アンロード処理
- セクションクラスはMonoBehaviourを継承しているので、ゲームオブジェクトにコンポーネント登録する前提で作られています。

### 移動時の処理の実行順


/a1 → /a2

| パス | 状態 |  |
| :-- | :-- | :-- |
| /a1 | Goto | 出発地点 | 
| /a1 | Unload |  | 
| /a2 | Load |  | 
| /a2 | Init | 到着地点 | 


/a1 → /a1/b2/c1

| パス | 状態 |  |
| :-- | :-- | :-- |
| /a1 | Goto | 出発地点  | 
| /a1/b2 | Load |  | 
| /a1/b2/c1 | Load |  | 
| /a1/b2/c1 | Init | 到着地点 | 

/a1/b2/c1 → /a1

| パス | 状態 |  |
| :-- | :-- | :-- |
| /a1/b2/c1 | Goto | 出発地点 | 
| /a1/b2/c1 | Unload |  | 
| /a1/b2 | Unload |  | 
| /a1 | Init | 到着地点 | 
    
## 使い方

以下のサンプルシーンを元に説明します。
- [Assets/Succession/Sample1.unity](https://github.com/usam1111/Succession/blob/master/Assets/Succession/Examples/Sample1.unity)

![](https://raw.githubusercontent.com/usam1111/Succession/master/Screenshots/succession_sample1.gif)

### 1. セクションを作成

各セクションクラスは、SectionBase を継承します。
下記のセクションテンプレート用クラスを複製して作成すると楽です。
- セクションテンプレート用クラス： [Assets/Succession/SectionTemplate.cs](https://github.com/usam1111/Succession/blob/master/Assets/Succession/Examples/SectionTemplate.cs)

セクションクラスの例： SectionA.cs

```cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Itach.Succession;

public class SectionA : SectionBase
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

}
```
**各「AtSectionLoad, AtSectionInit, AtSectionGoto, AtSectionUnload」の処理ではコルーチンを使いますが、IEnumerator.MoveNextでコルーチン終了を管理しているので、「yield return」を使わずにコルーチンを終わらせることで、フレームをまたがずにコルーチンが終了し、次の画面遷移の進行へ進めることもできます。**

サンプルシーンでは、SectionAの他にSectionBとSectionCも作ってゲームオブジェクトにコンポーネント登録します。
今回は分かりやすいようにシーン管理クラス「Sample1」とセクションクラス「SectionA」「SectionB」「SectionC」は同じゲームオブジェクトにコンポーネント登録しています。

![](https://raw.githubusercontent.com/usam1111/Succession/master/Screenshots/scenemanager_inspector.png)

### 2. セクションのツリー構造を作成

サンプルシーンでは、下記のようにシーン管理クラスでセクションのツリー構造を作成します。

```cs
using UnityEngine;
using Itach.Succession;

public class Sample1 : MonoBehaviour
{
    public Succession suc;

    void Start()
    {
        suc = new Succession(this);

        // 各セクションを登録
        var a1 = suc.Add(GetComponent<SectionA>().Initialize("a1"));
        var b1 = a1.Add(GetComponent<SectionB>().Initialize("b1"));
        var b2 = a1.Add(GetComponent<SectionB>().Initialize("b2"));
        var c1 = b2.Add(GetComponent<SectionC>().Initialize("c1"));
        var a2 = suc.Add(GetComponent<SectionA>().Initialize("a2"));

        // 全セクションへのパスを出力
        suc.OutputAllSectionPath();
        // 出力結果
        // [All section path] : /a1
        // [All section path] : /a1/b1
        // [All section path] : /a1/b2
        // [All section path] : /a1/b2/c1
        // [All section path] : /a2
    }
}
```
- SuccessionのAdd関数でセクションノードを登録します。
- Add関数は戻り値として登録したセクションノードを返します。
- セクションノードのAdd関数で子階層のセクションノードを登録していきます。
- Initialize("a1") の部分ではパスに使う文字列（ID）を登録します。

### 3. セクションの移動

SuccessionのGoto関数を実行し、引数に移動先のパス渡すことで、セクションの移動が開始されます。
セクション移動中でもGoto関数で別の移動先を指定することで移動先を切り替えることができます。
```cs
suc.Goto("/a1/b2");
```

相対パスで移動するには
- /a1 → /a1/b2
```cs
suc.Goto("./b2");
```
- /a1/b1 → /a1/b2
```cs
suc.Goto("../b2");
```
※セクション移動中に相対パスで目的地を変更する場合、最初のセクション移動開始地点からの相対パスが使われます。


### Tips

セクション移動の出発地を調べる
```cs
Debug.Log(suc.departedSectionId.path); // 出力結果の例：/a1/b1
```

セクション移動の現在地を調べる（AtSectionLoad、AtSectionInit、AtSectionGoto、AtSectionUnloadが実行されているセクションパス）
```cs
Debug.Log(suc.currentSectionId.path); // 出力結果の例：/a1/b1
```

セクション移動の目的地を調べる
```cs
Debug.Log(suc.destinedSectionId.path); // 出力結果の例：/a1/b1
```

セクションの移動状態の列挙型
```cs
switch (suc.state)
{
    case State.Idling: // 移動していない
    case State.Going: // 出発処理中
    case State.Unloading: // アンロード処理中
    case State.Loading: // ロード処理中
    case State.Initializing: // 到着処理中
        break;
}
```

セクション間を移動している状態かどうかのbool値
```cs
suc.isMoving
```

セクションパスからセクションを取得する
```cs
var section = (SectionC)suc.GetSection("/a1/b2/c1");
Debug.Log(section.id); // 出力結果：c1
```
※SectionCはサンプルシーン用のクラスです。

セクション移動開始のイベントを受け取る
```cs
suc.processStart += (sender, e) =>
{
    Debug.Log("Process Start");
};
```

セクション移動終了のイベントを受け取る
```cs
suc.processComplete += (sender, e) =>
{
    Debug.Log("Process Complete");
};
```
