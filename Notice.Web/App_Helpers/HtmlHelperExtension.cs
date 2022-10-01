using Notice.Data.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
	public static class HtmlHelperExtension
	{
		public static MvcHtmlString HtmlListTest<TModel>(this List<TModel> models, string name)
		{
			string resultValue = "";

			foreach (var item in models)
			{
				Type type = item.GetType();
				var value = type.GetProperty(name).GetValue(item, null);
			}
			
			return new MvcHtmlString(resultValue);
		}

		public static MvcHtmlString HtmlTest<TModel>(this TModel model, string name)
		{
			string resultValue = "";

			Type type = model.GetType();
			var value = type.GetProperty(name).GetValue(model, null);

			resultValue = value.ToString();

			return new MvcHtmlString(resultValue);
		}
		

		public static MvcHtmlString HtmlHidden<TModel>(this TModel entity, string name)
		{
			Type type = entity.GetType();
			var p = type.GetProperty(name);

			return new MvcHtmlString("name=\"" + name + "\" id=\"h" + name + "\" value=\"" + p.GetValue(entity, null) + "\"");
		}

		public static MvcHtmlString HtmlTextBox<TModel>(this TModel entity, string name)
		{
			Type type = entity.GetType();
			var p = type.GetProperty(name);

			return new MvcHtmlString("name=\"" + name + "\" id=\"tb" + name + "\" value=\"" + p.GetValue(entity, null) + "\"");
		}

		public static MvcHtmlString HtmlTextBoxes<TModel>(this TModel entity, string name)
		{
			Type type = entity.GetType();
			var p = type.GetProperty(name);

			return new MvcHtmlString("name=\"" + name + "\" value=\"" + p.GetValue(entity, null) + "\"");
		}

		public static MvcHtmlString HtmlButton<TModel>(this TModel entity, string name)
		{
			Type type = entity.GetType();
			var p = type.GetProperty(name);

			return new MvcHtmlString("name=\"" + name + "\" id=\"bt" + name + "\" value=\"" + p.GetValue(entity, null) + "\"");
		}

		public static MvcHtmlString HtmlSortClass<TModel>(this TModel model, string name, SortOrder order)
		{
			string returnStr = "";

            Type type = model.GetType();
            var property = type.GetProperty(name);

			if(property != null)
			{
                var value = property.GetValue(model, null);

                returnStr = "class='sorting_asc' aria-sort='ascending'";
                if (SortOrder.Desc == order)
                {
                    returnStr = "class='sorting_desc' aria-sort='descending'";
                }
            }
			

            return new MvcHtmlString(returnStr);
        }
	}

}