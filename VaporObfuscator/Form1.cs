﻿using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections;

namespace VaporObfuscator
{
    public partial class Form1 : Form
    {
        string[] attrib = { "YanoAttribute", "Xenocode.Client.Attributes.AssemblyAttributes.ProcessedByXenocode", "PoweredByAttribute", "ObfuscatedByGoliath", "NineRays.Obfuscator.Evaluation", "NetGuard", "dotNetProtector", "DotNetPatcherPackerAttribute", "DotNetPatcherObfuscatorAttribute", "DotfuscatorAttribute", "CryptoObfuscator.ProtectedWithCryptoObfuscatorAttribute", "BabelObfuscatorAttribute", "BabelAttribute", "AssemblyInfoAttribute" };
        int mov;
        int movX;
        int movY;
        bool nameprotection = false;
        bool fakeobfu = false;
        bool junkattribute = false;
        bool encryptstring = false;
        bool antide4dot = false;
        public Form1()
        {
            InitializeComponent();
        }

        public static byte[] encodeBytes(byte[] bytes, string pass)
        {
            byte[] XorBytes = Encoding.Unicode.GetBytes(pass);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= XorBytes[i % 16];
            }

            return bytes;
        }
        public static byte[] decryptBytes(byte[] bytes, String pass)
        {
            byte[] XorBytes = Encoding.Unicode.GetBytes(pass);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= XorBytes[i % 16];
            }

            return bytes;
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void ProtectName(AssemblyDef assembly, ModuleDef mod)
        {//rename module
            //rename type
            foreach (TypeDef type in mod.Types)
            {
                mod.Name = "ObfuscatedByVapor";
                if (type.IsGlobalModuleType || type.IsRuntimeSpecialName || type.IsSpecialName || type.IsWindowsRuntime || type.IsInterface)
                {
                    continue;
                }
                else
                {
                    foreach (PropertyDef property in type.Properties)
                    {
                        if (property.IsRuntimeSpecialName) continue;
                        property.Name = RandomString(20) + "俺ム仮 ｎｏ ｓｌｅｅｐ俺ム仮";
                    }
                    foreach (FieldDef fields in type.Fields)
                    {
                        fields.Name = RandomString(20) + "俺ム仮 ｎｏ ｓｌｅｅｐ俺ム仮";
                    }
                    foreach (EventDef eventdef in type.Events)
                    {
                        eventdef.Name = RandomString(20) + "俺ム仮 ｎｏ ｓｌｅｅｐ俺ム仮";
                    }
                    foreach (MethodDef method in type.Methods)
                    {
                        if (method.IsConstructor || method.IsRuntimeSpecialName || method.IsRuntime || method.IsStaticConstructor || method.IsVirtual) continue;
                         method.Name = RandomString(20);
                    }
                }
            }
        }

        public void fakeobfuscation(ModuleDefMD module)
        {
            for (int i = 0; i < attrib.Length; i++)
            {
                var fakeattrib = new TypeDefUser(attrib[i],attrib[i],module.CorLibTypes.Object.TypeDefOrRef);
                fakeattrib.Attributes = TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.WindowsRuntime;
                module.Types.Add(fakeattrib);

            }
        }

        public void junkattrib(ModuleDefMD module)
        {
            int number = System.Convert.ToInt32(textBox2.Text);
            for(int i = 0; i < number; i++)
            {
                var junkattribute = new TypeDefUser("俺ム仮 ｎｏ　ｓｌｅｅｐ　俺ム仮"+RandomString(20), "俺ム仮 ｎｏ　ｓｌｅｅｐ　俺ム仮"+RandomString(20), module.CorLibTypes.Object.TypeDefOrRef);
                module.Types.Add(junkattribute);
            }
        }

        public void encryptString(ModuleDef module)
        {
            foreach (TypeDef type in module.Types)
            {
                foreach(MethodDef method in type.Methods)
                {
                if (method.Body == null) continue;
                    method.Body.SimplifyBranches();
                    for (int i = 0; i < method.Body.Instructions.Count; i++)
                    {
                        if(method.Body.Instructions[i].OpCode == OpCodes.Ldstr)
                        {
                            string base64toencrypt = method.Body.Instructions[i].Operand.ToString();
                            string base64EncryptedString = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(base64toencrypt));
                            method.Body.Instructions[i].OpCode = OpCodes.Nop;
                            method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, module.Import(typeof(System.Text.Encoding).GetMethod("get_UTF8", new Type[] { }))));
                            method.Body.Instructions.Insert(i + 2, new Instruction(OpCodes.Ldstr, base64EncryptedString));
                            method.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Call, module.Import(typeof(System.Convert).GetMethod("FromBase64String", new Type[] { typeof(string) }))));
                            method.Body.Instructions.Insert(i + 4, new Instruction(OpCodes.Callvirt, module.Import(typeof(System.Text.Encoding).GetMethod("GetString", new Type[] { typeof(byte[]) }))));
                            i += 4;
                        }
                    }
                }
            }
        }

        public void antiDe4Dot(ModuleDefMD module)
        {
            Random rnd = new Random();
            InterfaceImpl Interface = new InterfaceImplUser(module.GlobalType);
            for (int i = 200; i < 300; i++)
            {
                TypeDef typedef = new TypeDefUser("", $"Form{i.ToString()}", module.CorLibTypes.GetTypeRef("System", "Attribute"));
                InterfaceImpl interface1 = new InterfaceImplUser(typedef);
                module.Types.Add(typedef);
                typedef.Interfaces.Add(interface1);
                typedef.Interfaces.Add(Interface);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.Filter = "Executables | *.*";
            openfiledialog.ShowDialog();
            textBox1.Text = openfiledialog.FileName;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            nameprotection = !nameprotection;
        }

        private void button2_Click(object sender, EventArgs e)
        { 
            AssemblyDef assembly = AssemblyDef.Load(textBox1.Text);
            ModuleContext modCtx = ModuleDefMD.CreateModuleContext();
            ModuleDefMD module = ModuleDefMD.Load(textBox1.Text, modCtx);
            if (antide4dot == true)
                {
                    antiDe4Dot(module);
                    module.Write(textBox3.Text + ".exe");
                }
                if (nameprotection == true)
                {
                    ProtectName(assembly, module);
                    module.Write(textBox3.Text + ".exe");
                }
                if (fakeobfu == true)
                {
                    fakeobfuscation(module);
                    module.Write(textBox3.Text + ".exe");
                }
                if(junkattribute == true)
                {
                    junkattrib(module);
                    module.Write(textBox3.Text + ".exe");
                }
                if(encryptstring == true)
                {
                    encryptString(module);
                    module.Write(textBox3.Text + ".exe");
                }
                MessageBox.Show("🍓  🎀  𝕌𝕎𝕌 𝕋𝕙𝕒𝕟𝕜𝕤 𝕗𝕠𝕣 𝕦𝕤𝕚𝕟𝕘 𝕍𝕒𝕡𝕠𝕣𝕆𝕓𝕗𝕦𝕤𝕔𝕒𝕥𝕠𝕣 𝕌𝕎𝕌  🎀  🍓", "🍓  🎀  𝕌𝕎𝕌 𝕋𝕙𝕒𝕟𝕜𝕤 𝕗𝕠𝕣 𝕦𝕤𝕚𝕟𝕘 𝕍𝕒𝕡𝕠𝕣𝕆𝕓𝕗𝕦𝕤𝕔𝕒𝕥𝕠𝕣 𝕌𝕎𝕌  🎀  🍓", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            fakeobfu = !fakeobfu;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            junkattribute = !junkattribute;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            encryptstring = !encryptstring;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = Screen.AllScreens[0].WorkingArea.Location;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_mousedown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.X;
            movY = e.Y;
        }

        private void panel2_mousemove(object sender, MouseEventArgs e)
        {
            if(mov == 1)
            {
                this.SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
            }
        }

        private void panel2_mouseup(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.Opacity = ((double)trackBar1.Value /trackBar1.Maximum);
        }

        private void checkBox4_CheckedChanged_1(object sender, EventArgs e)
        {
            antide4dot = !antide4dot;
        }
    }
}
