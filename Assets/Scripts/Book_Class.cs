// 目的の本の情報を保持（staticを付けることで他のクラスから参照可・・・グローバル変数に近い）
public static class SearchInformation {
    public static int floor = -1;
    public static string bookTitle = "";
    public static string bookAuthor = "";
    public static string bookCode = "";
}

// 各本が持つ要素．
// キーワードの検索結果は複数の本が返されるため，Bookクラスの配列
public class Book{
    public string url;
    public string name;
    public string author;
    public string publisher;
    public string position;
    public string existing;
}
