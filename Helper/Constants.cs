using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneDirect.Helper
{
    public static class Constants
    {
        //Shoulder
        public const int Sh_Flex_Current = 15;
        public const int Sh_Flex_Goal = 165;

        public const int Sh_ExRot_Current = 0;
        public const int Sh_ExRot_Goal = 220;

        public const int Sh_Abd_Current = 0;
        public const int Sh_Abd_Goal = 160;

        //Knee
        public const int Knee_Flex_Current = 90;
        public const int Knee_Flex_Goal = 132;

        public const int Knee_Ext_Current = 90;
        public const int Knee_Ext_Goal = -10;

        //Ankle
        public const int Ankle_Flex_Current = 5;
        public const int Ankle_Flex_Goal = 50;

        public const int Ankle_Ext_Current = 5;
        public const int Ankle_Ext_Goal = -20;

    }

    public static class ConstantsVar
    {
        public const int Admin = 0;
        public const int Support = 1;
        public const int Therapist = 2;
        public const int Provider = 3;
        public const int Installer = 4;
        public const int Patient = 5;

    }

    public static class AppointmentConstants
    {
        public const int Requested = 0;
        public const int SlotsReceived = 1;
        public const int SlotAccepted = 2;
        public const int Completed = 3;
        public const int RescheduleRequested = 4;
        public const int Expired = 5;
        public const int Cancelled = 6;
    }
    public static class AppointmentTypeConstants
    {
        public const int Therapist = 0;
        public const int Support = 1;
    }

    //public static class AppointmentSlotConstants
    //{
    //    public const string s0 = "12 AM - 1 AM";
    //    public const string s1 = "1 AM - 2 AM";
    //    public const string s2 = "2 AM - 3 AM";
    //    public const string s3 = "3 AM - 4 AM";
    //    public const string s4 = "4 AM - 5 AM";
    //    public const string s5 = "5 AM - 6 AM";
    //    public const string s6 = "6 AM - 7 AM";
    //    public const string s7 = "7 AM - 8 AM";
    //    public const string s8 = "8 AM - 9 AM";
    //    public const string s9 = "9 AM - 10 AM";
    //    public const string s10 = "10 AM - 11 AM";
    //    public const string s11 = "11 AM - 12 PM";
    //    public const string s12 = "12 PM - 1 PM";
    //    public const string s13 = "1 PM - 2 PM";
    //    public const string s14 = "2 PM - 3 PM";
    //    public const string s15 = "3 PM - 4 PM";
    //    public const string s16 = "4 PM - 5 PM";
    //    public const string s17 = "5 PM - 6 PM";
    //    public const string s18 = "6 PM - 7 PM";
    //    public const string s19 = "7 PM - 8 PM";
    //    public const string s20 = "8 PM - 9 PM";
    //    public const string s21 = "9 PM - 10 AM";
    //    public const string s22 = "10 AM - 11 AM";
    //    public const string s23 = "11 AM - 12 PM";

    //}


}
