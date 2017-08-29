using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ACAdapter
{
    public class ACAdapter
    {
        public string _outFile = string.Empty;
        public string _ip = string.Empty;

        public ACAdapter(string Ip,string outFile)
        {
            _outFile = outFile;
            _ip = Ip;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZGetState(char[] ip, char[] filePath);
        public int GetState()
        {
            return ZGetState(_ip.ToArray(), _outFile.ToArray());
        }

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetS1(char[] ip, int id, float t);
        public int SetS1(int id, float t)
        {
            return ZSetS1(_ip.ToArray(), id, t);
        }

        /// <summary>
        /// 设置风机状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t">t: 0：关，1：开</param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetS2(char[] ip, int id, float t);
        public int SetS2(int id, float t)
        {
            return ZSetS2(_ip.ToArray(), id, t);
        }

        /// <summary>
        /// 设置冷热状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t">t: 0：加热，1：制冷</param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetS3(char[] ip, int id, float t);
        public int SetS3(int id, float t)
        {
            return ZSetS3(_ip.ToArray(), id, t);
        }

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetH1(char[] ip, int id, float t);
        public int SetH1(int id, float t)
        {
            return ZSetH1(_ip.ToArray(), id, t);
        }

        /// <summary>
        /// 设置温控开关
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t">t: 0：关，1：开</param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetH2(char[] ip, int id, float t);
        public int SetH2(int id, float t)
        {
            return ZSetH2(_ip.ToArray(), id, t);
        }

        /// <summary>
        /// 设置风扇状态
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t">t: 0：低速，1：中速，2：高速，3：自动</param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetH3(char[] ip, int id, float t);
        public int SetH3(int id, float t)
        {
            return ZSetH3(_ip.ToArray(), id, t);
        }

        /// <summary>
        /// 设置系统模式
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="id"></param>
        /// <param name="t">t: 0：通风，1：加热，2：制冷</param>
        /// <returns></returns>
        [DllImport("GTTerminalDLL.dll")]
        private extern static int ZSetH4(char[] ip, int id, float t);
        public int SetH4(int id, float t)
        {
            return ZSetH4(_ip.ToArray(), id, t);
        }
    }
}
