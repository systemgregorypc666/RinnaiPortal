using System.ComponentModel;

namespace RinnaiPortal.Enums
{
    public enum IsoStatus
    {
        /// <summary>
        /// 未送出
        /// </summary>
        [Description("未送出")]
        U = 1,

        /// <summary>
        /// 待簽核
        /// </summary>
        [Description("待審核")]
        W = 2,

        /// <summary>
        /// 核准簽核
        /// </summary>
        [Description("已核准")]
        Y = 3,

        /// <summary>
        /// 拒絕簽核
        /// </summary>
        [Description("拒絕")]
        N = 4,
    }
}