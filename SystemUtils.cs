using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HS.Utils
{
    public static class SystemUtils
    {
        public static List<T> GetFlagsList<T>(T enumFlags) where T : Enum
        {
            List<T> result = new List<T>();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (enumFlags.HasFlag(value)) result.Add(value);
            }
            return result;
        }

        public static bool IsRunDotnet() => CurrentDotnetPath(out _) != null;
        public static string CurrentDotnetPath(out string executeModulePath)
        {
            var process = Process.GetCurrentProcess();
            executeModulePath = process.MainModule.FileName;
            return process.ProcessName.ToLower() == "dotnet" ? process.ProcessName : null;
        }
        
        public static Platform GetPlatform()
        {
            PlatformID pid = Environment.OSVersion.Platform;
            
            // Windows
            if (pid == PlatformID.Win32NT ||
                pid == PlatformID.Win32S ||
                pid == PlatformID.Win32Windows ||
                pid == PlatformID.WinCE)
            {
                return Platform.Windows;
            }
            
            // MacOS (Mono / .NET Standard 환경에서 Mac은 Unix로 감지됨)
            // macOS는 특정 경로로 판별
            if (pid == PlatformID.MacOSX || Directory.Exists("/System/Library/CoreServices"))
                return Platform.MacOS;

            // Linux
            // /proc 존재하면 거의 100% Linux
            if (Directory.Exists("/proc"))
                return Platform.Linux;

            // BSD 계열 (FreeBSD, OpenBSD, NetBSD)
            // 대표적인 BSD 시스템 파일로 판별
            if (File.Exists("/etc/freebsd-update.conf") ||   // FreeBSD
                File.Exists("/etc/rc.conf"))                 // FreeBSD / OpenBSD 공통
            {
                return Platform.Unix; // BSD → Unix 로 통합
            }
            
            return Platform.Unknown;
        }
        
        /*
        public static List<T> GetFlagsList<T>(T enumFlags) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T는 enum 타입이어야 합니다.", nameof(enumFlags));
            var flagsEnum = (Enum)(object)enumFlags;
            List<T> result = new List<T>();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                var valueEnum = (Enum)(object)value;
                if (flagsEnum.HasFlag(valueEnum)) result.Add(value);
            }
            return result;
        }
        */
    }
    
    public enum Platform
    {
        Unknown,
        Windows,
        Linux,
        MacOS,
        Unix,
    }
}
