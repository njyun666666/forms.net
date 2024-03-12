using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Enums
{
    public enum FlowStepWhenTypeEnum
    {
        /// <summary>
        /// 拋出例外
        /// </summary>
        exception,
        /// <summary>
        /// 略過
        /// </summary>
        pass,
        /// <summary>
        /// 簽核
        /// </summary>
        sign,
        /// <summary>
        /// 往上找
        /// </summary>
        find_parent
    }
}
