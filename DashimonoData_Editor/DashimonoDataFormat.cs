namespace DashimonoData_Editor
{
    /// <summary>
    /// 出し物データの共通形式
    /// </summary>
    public class DashimonoDataFormat
    {
        /// <summary>
        /// 学年・クラス
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// 出し物の名前
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 受け入れ可能人数
        /// </summary>
        public int OKNinzu { get; set; }
    }
}
