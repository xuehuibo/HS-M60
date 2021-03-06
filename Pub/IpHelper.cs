﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pub
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    /**/
    /// <summary>
    /// 用于读取和配置以太网卡的IP地址信息
    /// </summary>
    public static class IpHelper
    {
        private const int OPEN_EXISTING = 3;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int IOCTL_NDIS_REBIND_ADAPTER = 1507374;
        private const int IOCTL_NDIS_GET_ADAPTER_NAMES = 1507386;

        [DllImport("Coredll.dll", EntryPoint = "CreateFile")]
        private static extern int CreateFile(
                string lpFileName,
                int dwDesiredAccess,
                int dwShareMode,
                int lpSecurityAttributes,
                int dwCreationDisposition,
                int dwFlagsAndAttributes,
                int hTemplateFile
            );

        [DllImport("Coredll.dll", EntryPoint = "DeviceIoControl")]
        private static extern int DeviceIoControl(
                int hDevice,
                int dwIoControlCode,
                string lpInBuffer,
                int nInBufferSize,
                string lpOutBuffer,
                int nOutBufferSize,
                int lpBytesReturned,
                int lpOverlapped
            );

        [DllImport("Coredll.dll", EntryPoint = "DeviceIoControl")]
        private static extern int DeviceIoControl2(
                int hDevice,
                int dwIoControlCode,
                string lpInBuffer,
                int nInBufferSize,
                string lpOutBuffer,
                int nOutBufferSize,
                ref int lpBytesReturned,
                int lpOverlapped
            );

        [DllImport("Coredll.dll", EntryPoint = "CloseHandle")]
        private static extern int CloseHandle(int hObject);

        /**/
        /// <summary>
        /// 保存以太网卡IP地址信息的注册表节点
        /// </summary>
        private const string TcpIpRegKeyName = @"HKEY_LOCAL_MACHINE\Comm\{0}\Parms\TcpIp";

        /**/
        /// <summary>
        /// IP地址注册表项名称
        /// </summary>
        private const string IpAddressItem = "IpAddress";

        /**/
        /// <summary>
        /// 子网掩码的注册表项名称
        /// </summary>
        private const string SubnetMaskItem = "Subnetmask";

        /**/
        /// <summary>
        /// 默认网关的注册表项名称
        /// </summary>
        private const string DefaultGatewayItem = "DefaultGageway";

        /**/
        /// <summary>
        /// 以太网卡的设备文件名称
        /// </summary>
        private const string EtherCardFileName = "NDS0:";

        /**/
        /// <summary>
        /// 以太网卡的名称
        /// </summary>
        private static string EtherCardName = string.Empty;

        /**/
        /// <summary>
        /// 保存真实的以太网卡IP地址信息的注册表节点
        /// </summary>
        private static string RealTcpIpRegKeyName = string.Empty;

        /**/
        /// <summary>
        /// 在注册表中对IP信息进行修改后，禁用网卡然后重启网卡以应用修改
        /// </summary>
        public static bool ApplyIpAddress()
        {
            int hNdis = CreateFile(EtherCardFileName, 0, 0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, INVALID_HANDLE_VALUE);
            if (hNdis == INVALID_HANDLE_VALUE)
            {
                return false;
            }

            // Send the device command.
            if (DeviceIoControl(hNdis, IOCTL_NDIS_REBIND_ADAPTER, EtherCardName, EtherCardName.Length * 2 + 2, null, 0, 0, 0) == 0)
            {
                return false;
            }

            CloseHandle(hNdis);
            return true;
        }

        /**/
        /// <summary>
        /// 获取子网掩码
        /// </summary>
        /// <returns></returns>
        public static string GetSubnetMask()
        {
            return GetRegValue(SubnetMaskItem, "0.0.0.0");
        }

        /**/
        /// <summary>
        /// 设置子网掩码
        /// </summary>
        /// <param name="subnetMask"></param>
        /// <returns></returns>
        public static bool SetSubnetMask(string subnetMask)
        {
            if (string.IsNullOrEmpty(subnetMask))
            {
                throw new ArgumentNullException(subnetMask);
            }

            return SetRegValue(SubnetMaskItem, subnetMask);
        }


        /**/
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress()
        {
            return GetRegValue(IpAddressItem, "0.0.0.0");
        }


        /**/
        /// <summary>
        /// 设置Ip地址
        /// </summary>
        /// <param name="ip"></param>
        public static bool SetIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException("ip");

            return SetRegValue(IpAddressItem, ip);
        }



        /**/
        /// <summary>
        /// 获得网卡的名称
        /// </summary>
        /// <returns></returns>
        private static string GetEtherCardName()
        {
            string names = new string(' ', 255);
            int bytes = 0;

            int fileHandle = CreateFile(EtherCardFileName, 0, 0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, INVALID_HANDLE_VALUE);
            if (fileHandle == INVALID_HANDLE_VALUE)
            {
                return string.Empty;
            }

            if (DeviceIoControl2(fileHandle, IOCTL_NDIS_GET_ADAPTER_NAMES, null, 0, names, 255, ref bytes, 0) == 0)
            {
                return string.Empty;
            }

            int index = names.IndexOf('\0');
            string result = names.Substring(0, index);
            return result;
        }


        /**/
        /// <summary>
        /// 获取注删表项的值
        /// </summary>
        private static string GetRegValue(string regItemName, string defaultValue)
        {
            if (string.IsNullOrEmpty(EtherCardName))
            {
                EtherCardName = GetEtherCardName();
                RealTcpIpRegKeyName = string.Format(TcpIpRegKeyName, EtherCardName);
            }

            if (!string.IsNullOrEmpty(EtherCardName))
            {
                try
                {
                    object value = Registry.GetValue(RealTcpIpRegKeyName, regItemName, defaultValue);
                    if (value != null)
                    {
                        if (value.GetType() == typeof(string))
                        {
                            return (string)value;
                        }
                        if (value.GetType() == typeof(string[]))
                        {
                            return ((string[])value)[0];
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            return defaultValue;
        }

        /**/
        /// <summary>
        /// 设置注册表项的值
        /// </summary>
        private static bool SetRegValue(string regItemName, string value)
        {
            if (string.IsNullOrEmpty(EtherCardName))
            {
                EtherCardName = GetEtherCardName();
                RealTcpIpRegKeyName = string.Format(TcpIpRegKeyName, EtherCardName);
            }
            try
            {
                Registry.SetValue(RealTcpIpRegKeyName, regItemName, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
