using System;
using System.Collections.Generic;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public static class Consts
    {
        public static string ClassTemplate = "\\TextFiles\\POCO\\poco.txt";
        public static string ParentCodeBLLTemplate = "\\TextFiles\\BLL\\code_parent.txt";
        public static string ParentBLLTemplate = "\\TextFiles\\business_object_parent.txt";
        public static string DaoTemplate = "\\TextFiles\\data_access_object.txt";
        public static string DaoFunctionsTemplate = "\\TextFiles\\DAO";
        public static string DaoGetTemplate = "\\TextFiles\\DAO\\get.txt";
        public static string DaoGetByIdTemplate = "\\TextFiles\\DAO\\get_id.txt";
        public static string DaoSaveTemplate = "\\TextFiles\\DAO\\save.txt";
        public static string DaoUpdateTemplate = "\\TextFiles\\DAO\\update.txt";
        public static string DaoDeleteTemplate = "\\TextFiles\\DAO\\delete.txt";
        public static string ParentBLLGetByIDTemplate = "business_object_parent_get_id.txt";
        public static string Namspace = "$namespace$";
        public static string Table = "$table$";
        public static string TableLowercase = "$lowercase_table$";
        public static string Sql = "$sql$";
        public static string CodeGenFolderName = "~~~";
        public static string Functions = "$functions$";
        public static string Parameters = "$parameters$";
        public static string Arguments = "$arguments$";
        public static string CommaPlusArguments = ", $arguments$";
        public static string Assignments = "$assignments$";
        public static string Poco = "$poco$";
        public static string BllFunctionName = "$function_name$";
        public static string PrimaryKey = "$primary_key$";
        public static string ReturnType = "$return_type$";
        public static string TransactionManager = "ITransactionManager";
        public static string DefaultValue = "$default_value$";

        public static string GuiEntityForm = "\\TextFiles\\GUI\\entity_form.txt";
        public static string GuiEntityFormDesigner = "\\TextFiles\\GUI\\entity_form_designer.txt";

        public static string ControlsToObject = "$controls_to_object$";
        public static string ObjectToControls = "$object_to_controls$";
        public static string ClearControls = "$clear_controls$";
    }
}
