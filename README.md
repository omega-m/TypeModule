# Name TypeModule
Unityでタイピングゲームを開発する際に必要となる、共通処理を担当するモジュールです。


このモジュールを使う事で、以下の機能を素早く実装することができます。  
* TextBoxを使わずに、キーボードの入力から日本語(英語)の文字列を生成及び取得。
* キーボードの入力から、指定された文字列が正しく打ててるかを確認するシステム。(ありがちな、お題の文字を真似して打たせるタイピングゲーム形式のシステム部分)
 
# Installation
以下のパッケージを、Unityのプロジェクトにインポートしてください。  
[TypeModule_v1_0_0](https://github.com/omega-m/TypeModule/blob/main/Packages/TypeModule_v1_0_0.unitypackage)
  
# Usage
以下のスクリプトを、管理クラスのコンポーネントとして割り当ててください。  
Assets/TypeModule/TypeModule.cs  

TypeModuleを使用したいインスタンスに、以下のようなコードを加えてください。
    
    //管理クラスに割り当てたスクリプトコンポーネントを取得
    module = GetComponent<TypeModule>(); 
        
    //=======================================
    //文字列生成シミュレーションモードに変更
    module.Mode = TypeModule.MODE.INPUT;
    
    //モジュールから状態を取得
    Debug.Log(module.Str);
    Debug.Log(module.PrevChar);
    Debug.Log(module.StrRaw);
    
    //モードを変更
    module.IsInputEng = true      //英語入力状態へ
    module.IsKana     = true;     //かな入力状態へ
    module.EnabledBS  = true;     //BSで文字を消せるかどうか
    
    //プログラムから文字列を操作
    module.Enter();             //変換確定前の文字列を確定
    module.BackSpace();         //末尾から1文字削除
    module.Clear();             //全ての文字を削除
    
    //イベントリスナを追加し、文字列に変更があった時にGUIテキストを修正
    module.AddEventListenerOnChange(onChange);
    
            ...
    
    public Text testInput;
    public Text testInputRaw;
    
    private void onChange(InputEmulatorResults res) {
        Debug.Log("onChange");
        
        testInput.text = res.StrInput;
        testInputRaw.text = res.StrRawInput;
    }
    
    
    //イベントリスナを追加し、文字が打たれた時にサウンドを再生
    public AudioSource audioSource;
    public AudioClip typeSound;
    public AudioClip bsSound;
    public AudioClip enterSound;
    
            ...
    
    
    module.AddEventListenerOnInput(onInput);
    audioSource = GetComponent<AudioSource>();
    
            ...
    
    private void onInput(InputEmulatorResults res){
        Debug.Log("onInput");
        switch(res.InputType){
            case InputEmulatorResults.INPUT_TYPE.INPUT:
                audioSource.PlayOneShot(typeSound);
                break;
            case InputEmulatorResults.INPUT_TYPE.BS:
                audioSource.PlayOneShot(bsSound);
                break;
            case InputEmulatorResults.INPUT_TYPE.ENTER:
                audioSource.PlayOneShot(enterSound);
                break;
        }
    }
    
    
    //=======================================
    //指定された文字列が正しく打ててるか、比較するモードに変更
    module.Mode = TypeModule.MODE.COPY;
    
    //モードの変更
    module.IsKana = true;                  /かな入力入力状態へ
    module.IsCaseSensitive = true;         //英語の大文字と小文字入力を区別
    
    //比較対象の文字列をセット(内部の初期化も同時に行われます)
    module.TargetStr = "こちらは、たいぴんぐするぶんしょうです。";
    
    
    //直前に入力された文字を取得
    Debug.Log(module.PrevCorrectChar);         //正しく入力された場合
    Debug.Log(module.PrevMissChar);            //ミス入力の場合
    
    //パラメータにアクセス
    Debug.Log(module.CorrectNum);              //正しくタイプした数
    Debug.Log(module.CorrectCharNum);          //正しく打てた文字数
    Debug.Log(module.MissNum);                 //ミスタイプした数
    Debug.Log(module.IsComplete);              //指定文字列を打ち切ったか
         
    //イベントリスナを追加し、文字列に変更があった時にGUIテキストを修正
    module.AddEventListenerOnInput(onInput);
            
        ...
        
    public Text testInput;
    public Text testInputRaw;
    
    private void onInput(CopyInputCheckerResults res) {
        Debug.Log("onInput");
        testInput.text = res.StrDone + " " + res.StrCurrent + " " + res.StrYet;
        testInputRaw.text = res.StrDoneRaw + " " + res.StrCurrentRaw + " " + res.StrYetRaw;
    }
    
    
    //イベントリスナを追加し、文字が打たれた時にサウンドを再生
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip missSound;
    
        ...
    
    module.AddEventListenerOnCorrect(onCorrect);
    module.AddEventListenerOnMiss(onMiss);
    audioSource = GetComponent<AudioSource>();
    
        ...
    
    private void onCorrect(CopyInputCheckerResults res){
        Debug.Log("onCorrect");
        audioSource.PlayOneShot(correctSound);
    }
    
    private void onMiss(CopyInputCheckerResults res){
        Debug.Log("onMiss");
        audioSource.PlayOneShot(missSound);
    }
    
# DEMO

作成中です。

# Note

TypeModuleでは、CSVファイル[.csv]を使う事で、キーボードの配置レイアウトを簡単に変更できるようになっております。  

デフォルトでは以下のレイアウトが入っています。  
* 標準配列(qwerty)
* 英語圏配列(US)
* JISかな配列

独自でキーボードの配置レイアウトを登録したい場合は、以下のフォルダに入っているデータを参考に追加し、TypeModule.csから該当のCSVファイルを参照するようにしてください。  
Assets\Resources\TypeModule\data

# Author
omega
 
# License
"TypeModule" is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).
