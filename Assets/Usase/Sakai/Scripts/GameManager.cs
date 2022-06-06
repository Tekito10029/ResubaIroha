using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // パラメーターを追加
    private Queue<char> _charQueue;
    private const char SEPARATE_PAGE = '&';
    private const char SEPARATE_MAIN_START = '「';
    private const char SEPARATE_MAIN_END = '」';
    private Queue<string> _pageQueue;
    [SerializeField]
    private Text mainText;
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private float captionSpeed = 0.2f;
    [SerializeField]
    private GameObject nextPageIcon;
    private string _text = "ひろゆき「Hello,World」&ひろゆき「それってあなたの感想ですよね」";

    // Start is called before the first frame update
    void Start()
    {
        ReadLine(_text);
        OutputChar();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        //左クリックで反応
        if (Input.GetMouseButtonDown(0)) OnClick();
    }

    //文字表示
    private void ReadLine(string text)
    {
        // '「'の位置で文字列を分ける
        string[] ts = text.Split(SEPARATE_MAIN_START);
        // 分けたときの最初の値、つまり"ひろゆき"が代入される
        string name = ts[0];
        // 分けたときの次の値、つまり"Hello,World!」"が代入されるので
        // 最後の閉じ括弧を削除して代入
        string main = ts[1].Remove(ts[1].LastIndexOf(SEPARATE_MAIN_END));
        nameText.text = name;
        mainText.text = "";
        _charQueue = SeparateString(main);
        // コルーチンを呼び出す
        StartCoroutine(ShowChars(captionSpeed));
    }

    //文字を切り分け並べる
    private Queue<char> SeparateString(string str)
    {
        // 文字列をchar型の配列にする = 1文字ごとに区切る
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        // foreach文で配列charsに格納された文字を全て取り出し
        // キューに加える
        foreach (char c in chars) charQueue.Enqueue(c);
        return charQueue;
    }

    //一文字ずつ表示
    private bool OutputChar()
    {
        // キューに何も格納されていなければfalseを返す
        if (_charQueue.Count <= 0) 
        {
            nextPageIcon.SetActive(true);
            return false;
        }
        // キューから値を取り出し、キュー内からは削除する
        mainText.text += _charQueue.Dequeue();
        return true;
    }

    //遅らせ表示
    private IEnumerator ShowChars(float wait)
    {
        // OutputCharメソッドがfalseを返す(=キューが空になる)までループする
        while (OutputChar())
            // wait秒だけ待機
            yield return new WaitForSeconds(wait);
        // コルーチンを抜け出す
        yield break;
    }

    //全文表示
    private void OutputAllChar()
    {
        // コルーチンをストップ
        StopCoroutine(ShowChars(captionSpeed));
        // キューが空になるまで表示
        while (OutputChar()) ;
        nextPageIcon.SetActive(true);
    }

    //文字列を指定した区切り文字ごとに区切り、キューに格納したものを返す
    private Queue<string> SeparateString(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }

    //初期化
    private void Init()
    {
        _pageQueue = SeparateString(_text, SEPARATE_PAGE);
        ShowNextPage();
    }

    //次のページを表示する
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0)
        {
            return false;
        }
        // オブジェクトの表示/非表示を設定する
        nextPageIcon.SetActive(false);
        ReadLine(_pageQueue.Dequeue());
        return true;
    }

    //クリックした時の処理
    private void OnClick()
    {
        if (_charQueue.Count > 0) OutputAllChar();
        else
        {
            if (!ShowNextPage())
                // UnityエディタのPlayモードを終了する
                UnityEditor.EditorApplication.isPlaying = false;
        }
    }

}
