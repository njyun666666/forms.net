using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsNet.Enums
{
    /// <summary>
    /// 簽核流程步驟 TB_Flow_Step_Type
    /// </summary>
    public enum StepTypeEnum
    {
        /// <summary>
        /// 申請表單
        /// </summary>
        NewApplicant = 0,
        /// <summary>
        /// 開始
        /// </summary>
        Start = 1,
        /// <summary>
        /// 同意結束
        /// </summary>
        EndAgree = 2,
        /// <summary>
        /// 駁回結束
        /// </summary>
        EndReject = 3,
        /// <summary>
        /// 申請人
        /// </summary>
        ApplicantSelf = 4,
        /// <summary>
        /// 申請人路線
        /// </summary>
        ApplicantRoute = 5,
        /// <summary>
        /// 申請人部門主管
        /// </summary>
        ApplicantDepartment = 6,
        /// <summary>
        /// 申請人直屬主管
        /// </summary>
        ApplicantAllBoss = 7,
        /// <summary>
        /// 簽核人部門主管
        /// </summary>
        LastApproverDepartment = 8,
        /// <summary>
        /// 指定人(串簽)
        /// </summary>
        SpecificMember = 9,
        /// <summary>
        /// 指定對象(會簽)
        /// </summary>
        DesignateList = 10,
        /// <summary>
        /// 指定角色(會簽)
        /// </summary>
        SpecificRole = 11,
        /// <summary>
        /// 自定人(串簽)
        /// </summary>
        MemberByFunction = 12,
        /// <summary>
        /// 自定角色(會簽)
        /// </summary>
        RoleByFunction = 13,
        /// <summary>
        /// 指定部門(指定)
        /// </summary>
        GotoSpecificDepartment = 14,
        /// <summary>
        /// 指定部門(循序)
        /// </summary>
        RouteSpecificDepartment = 15,
        /// <summary>
        /// 指定職級主管(指定)
        /// </summary>
        GotoSpecificJobGrade = 16,
        /// <summary>
        /// 指定職級主管(循序)
        /// </summary>
        RouteSpecificJobGrade = 17,
        /// <summary>
        /// 指定職稱主管(指定)
        /// </summary>
        GotoSpecificJobTitle = 18,
        /// <summary>
        /// 指定職稱主管(循序)
        /// </summary>
        RouteSpecificJobTitle = 19,
        /// <summary>
        /// 核決權限表(指定)
        /// </summary>
        GotoAuthorizationChart = 20,
        /// <summary>
        /// 核決權限表(循序)
        /// </summary>
        RouteAuthorizationChart = 21,
        /// <summary>
        /// 條件式
        /// </summary>
        Decision = 22,
        /// <summary>
        /// 加簽(串簽)
        /// </summary>
        AddApprover = 23,
        /// <summary>
        /// 加簽(會簽)
        /// </summary>
        AddApproverCountersign = 24,
        /// <summary>
        /// 指定部門層級(循序)
        /// </summary>
        RouteSpecificDeptLevel = 25,
        /// <summary>
        /// 指定部門層級(指定)	
        /// </summary>
        GotoSpecificDeptLevel = 26,
        /// <summary>
        /// 未成立結束
        /// </summary>
        EndNotEstablished = 27,
        /// <summary>
        /// 作廢結束
        /// </summary>
        EndInvalidate = 28,
        /// <summary>
        /// 結案結束
        /// </summary>
        EndCaseCompleted = 29
    }
    
}
