using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyNhaThuoc.Areas.KhachHang.Services
{
    public static class RenderViewService
    {
        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            // Nếu viewName không có giá trị, sử dụng action hiện tại
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            // Tạo HttpContext và ActionContext từ ControllerContext
            var actionContext = new ActionContext(controller.HttpContext, controller.RouteData, controller.ControllerContext.ActionDescriptor);

            // Đặt Model vào ViewData
            controller.ViewData.Model = model;

            // Sử dụng ViewEngine để tìm và render view
            using (var writer = new StringWriter())
            {
                var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine.FindView(actionContext, viewName, !partial);

                if (!viewResult.Success)
                {
                    throw new Exception($"Could not find view '{viewName}'");
                }

                // Render view vào writer (StringWriter) để lấy HTML
                var viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                // Trả về HTML render được
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
