using System.ComponentModel;

namespace RinnaiPortal.Enums
{
    /// <summary>
    /// 記錄所有調查表格式
    /// </summary>
    public enum TrainingTableTypeEnum
    {
        /// <summary>
        /// 未設定
        /// </summary>
        [Description("未設定")]
        NotSet = 0,

        /// <summary>
        /// 學員意見調查表
        /// </summary>
        [Description("學員意見調查表")]
        TABLE01 = 1,

        /// <summary>
        /// 受訓心得報告表
        /// </summary>
        [Description("受訓心得報告表")]
        TABLE02 = 2,

        /// <summary>
        /// 員工訓練成效評核追蹤表
        /// </summary>
        [Description("員工訓練成效評核追蹤表")]
        TABLE03 = 3,

        /// <summary>
        /// 教育訓練成效調查表
        /// </summary>
        [Description("教育訓練成效調查表")]
        TABLE04 = 4,
    }
}