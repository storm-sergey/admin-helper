using System;

namespace AdminHelper
{
    /// <summary>
    /// Global variables
    /// </summary>
    public static class Globals
    {
        #region Rolf
        public static readonly string DOMAIN = "rolfnet";
        public static readonly string ROLF_DNS_SUFFIX = "int.rolfcorp.ru";
        public static readonly string SERVICE_DESK_EMAIL = "sd@rolf.ru";
        #endregion

        #region RolfAPI
        public static readonly string TICKETS_ENDPOINT = null;
        public static readonly string PRINT_SERVERS_ENDPOINT = null;
        public static readonly string PRINTERS_ENDPOINT = null;
        public static readonly string SUBNETS_ENDPOINT = null;
        public static readonly string LOG_ENDPOINT = null;
        #endregion

        #region RolfShare
        public static readonly string SHARE = @"\\1\IT_Distrib\";
        public static readonly string SHARE_APP = $@"{SHARE}AdminHelper\";
        public static readonly string CONFIG = $@"{SHARE_APP}config\";

        public static readonly string SUBNETS_FILE = $@"{CONFIG}Subnets.json";
        public static readonly string TICKETS_FILE = $@"{CONFIG}Ticket_templates.json";
        public static readonly string PRINT_SERVERS_FILE = $@"{CONFIG}Print_servers.json";
        public static readonly string PRINTERS_FILE = $@"{CONFIG}Printers_rooms.json";
        #endregion

        #region App
        public static readonly string ICONS_COLOR = "#212121";
        public static readonly string APP_DATA = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\AdminHelper";
        public static readonly string LOG_FILE = $@"{APP_DATA}\Log.txt";
        #endregion
    }
}