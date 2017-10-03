using Cgtor.Lib.Models;

namespace Cgtor.Lib.Controllers
{
    public interface IModelPropertyController
    {
        string GetNullableMarkIfApplicable(ModelProperty modelProperty);
    }

    public class ModelPropertyController : IModelPropertyController
    {
        public string GetNullableMarkIfApplicable(ModelProperty modelProperty)
        {
            if ((modelProperty.IsNullable) && (modelProperty.PropertyDataType != "string"))
            {
                return "?";
            }
            return string.Empty;
        }
    }
}