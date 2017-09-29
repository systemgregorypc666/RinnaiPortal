using System.ComponentModel;

namespace RinnaiPortal.Enums
{
    /// <summary>
    /// 簽核狀態列舉
    /// </summary>
    public enum SignTypeEnum
    {
        /// <summary>
        /// 未送出
        /// </summary>
        [Description("未送出")]
        NOTSENT = 1,

        /// <summary>
        /// 待簽核
        /// </summary>
        [Description("待簽核")]
        READYSIGN = 2,

        /// <summary>
        /// 核准
        /// </summary>
        [Description("核准")]
        APPROVAL = 3,

        /// <summary>
        /// 駁回
        /// </summary>
        [Description("駁回")]
        REFUSAL = 4,

        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        CANCEL = 5,

        /// <summary>
        /// 歸檔
        /// </summary>
        [Description("歸檔")]
        ARCHIVE = 6,
    }
}