using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Admin.ViewModels
{
    public class ApiReturnModel
    {
        #region 内部类定义
        public class PageInfo         //分页数据。如无分页，可为空
        {
            public int page;
            public int pageCount;
            public int pageSize;

        }

        public class ExceptionInfo       //异常详情。入无异常，可为空
        {
            public string message;
            public string stackTrace;
            public object data;

        }
        #endregion

        public int result;        //1 表示成功，0表示失败，-1表示异常，其它自定义
        public object data;         //接口返回数据，可以是基本类型、object、数组
        public string message;     //字符串类型，操作结果说明，比如错误提示
        public int affectRows; //影响行数，仅对POST类会修改数据的接口有效

        public PageInfo pageInfo;
        public ExceptionInfo Exception;
    }
}
