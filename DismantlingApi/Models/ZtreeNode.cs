namespace DismantlingApi.Models
{
    /// <summary>
    /// ztree 用到的类
    /// </summary>
    public class ZtreeNode
    {
        #region ztree 用到的字段

        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string pId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        public bool nocheck { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool chkDisabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool @checked { get; set; }


        public bool open { get; set; }

        #endregion
    }
}
