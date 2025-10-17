using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.Intrinsics.X86.Avx10v1;

namespace Caldera
{
    using System;

    public class WaterProperties
    {
        // Constants
        private const double tc_water = 647.096;     // Critical temperature [K]
        private const double pc_water = 220.64;       // Critical pressure [bar]
        private const double dc_water = 322.0;        // Critical density [kg/m³]
        private const double rgas_water = 461.526;    // Specific gas constant for water [J/(kg·K]

        // Coefficient arrays
        private readonly double[] nreg1 = new double[35];
        private readonly int[] ireg1 = new int[35];
        private readonly int[] jreg1 = new int[35];

        private readonly double[] nreg2 = new double[44];
        private readonly int[] ireg2 = new int[44];
        private readonly int[] jreg2 = new int[44];

        private readonly double[] nreg3 = new double[41];
        private readonly int[] ireg3 = new int[41];
        private readonly int[] jreg3 = new int[41];

        private readonly double[] nreg4 = new double[11];
        private readonly double[] nbound = new double[6];
        private readonly double[] n0thcon = new double[4];
        private readonly double[,] nthcon = new double[5, 6]; // [i, j] -> (0 to 4), (0 to 5)
        private readonly double[] n0visc = new double[4];  // i from 0 to 3
        private readonly int[] ivisc = new int[20];         // i from 1 to 19
        private readonly int[] jvisc = new int[20];         // i from 1 to 19
        private readonly double[] nvisc = new double[20];   // i from 1 to 19
        private readonly int[] j0reg2 = new int[10]; // Índices de 1 a 9
        private readonly double[] n0reg2 = new double[10]; // Índices de 1 a 9
        public WaterProperties()
        {
            InitFieldsreg1();
            InitFieldsreg2();
            InitFieldsreg3();
            InitFieldsreg4();
            InitFieldsbound();
        }

        #region Region Initialization

  

        private void InitFieldsvisc()
        {
            // Initialize n0visc (base terms)
            n0visc[0] = 1.0;
            n0visc[1] = 0.978197;
            n0visc[2] = 0.579829;
            n0visc[3] = -0.202354;

            // Initialize ivisc (exponent for delta)
            ivisc[1] = 0; ivisc[2] = 0; ivisc[3] = 0; ivisc[4] = 0;
            ivisc[5] = 1; ivisc[6] = 1; ivisc[7] = 1; ivisc[8] = 1;
            ivisc[9] = 2; ivisc[10] = 2; ivisc[11] = 2; ivisc[12] = 3;
            ivisc[13] = 3; ivisc[14] = 3; ivisc[15] = 3; ivisc[16] = 4;
            ivisc[17] = 4; ivisc[18] = 5; ivisc[19] = 6;

            // Initialize jvisc (exponent for tau)
            jvisc[1] = 0; jvisc[2] = 1; jvisc[3] = 4; jvisc[4] = 5;
            jvisc[5] = 0; jvisc[6] = 1; jvisc[7] = 2; jvisc[8] = 3;
            jvisc[9] = 0; jvisc[10] = 1; jvisc[11] = 2; jvisc[12] = 0;
            jvisc[13] = 1; jvisc[14] = 2; jvisc[15] = 3; jvisc[16] = 0;
            jvisc[17] = 3; jvisc[18] = 1; jvisc[19] = 3;

            // Initialize nvisc (coefficients)
            nvisc[1] = 0.5132047;
            nvisc[2] = 0.3205656;
            nvisc[3] = -0.7782567;
            nvisc[4] = 0.1885447;
            nvisc[5] = 0.2151778;
            nvisc[6] = 0.7317883;
            nvisc[7] = 1.241044;
            nvisc[8] = 1.476783;
            nvisc[9] = -0.2818107;
            nvisc[10] = -1.070786;
            nvisc[11] = -1.263184;
            nvisc[12] = 0.1778064;
            nvisc[13] = 0.460504;
            nvisc[14] = 0.2340379;
            nvisc[15] = -0.4924179;
            nvisc[16] = -0.0417661;
            nvisc[17] = 0.1600435;
            nvisc[18] = -0.01578386;
            nvisc[19] = -0.003629481;
        }


        private void InitFieldsThCon()
        {
            // Initialize coefficients for thermal conductivity (n0thcon)
            n0thcon[0] = 1.0;
            n0thcon[1] = 6.978267;
            n0thcon[2] = 2.599096;
            n0thcon[3] = -0.998254;

            // Initialize nthcon[i,j] matrix
            // i ranges from 0 to 4
            // j ranges from 0 to 5

            // i = 0
            nthcon[0, 0] = 1.3293046;
            nthcon[0, 1] = -0.40452437;
            nthcon[0, 2] = 0.2440949;
            nthcon[0, 3] = 0.018660751;
            nthcon[0, 4] = -0.12961068;
            nthcon[0, 5] = 0.044809953;

            // i = 1
            nthcon[1, 0] = 1.7018363;
            nthcon[1, 1] = -2.2156845;
            nthcon[1, 2] = 1.6511057;
            nthcon[1, 3] = -0.76736002;
            nthcon[1, 4] = 0.37283344;
            nthcon[1, 5] = -0.1120316;

            // i = 2
            nthcon[2, 0] = 5.2246158;
            nthcon[2, 1] = -10.124111;
            nthcon[2, 2] = 4.9874687;
            nthcon[2, 3] = -0.27297694;
            nthcon[2, 4] = -0.43083393;
            nthcon[2, 5] = 0.13333849;

            // i = 3
            nthcon[3, 0] = 8.7127675;
            nthcon[3, 1] = -9.5000611;
            nthcon[3, 2] = 4.3786606;
            nthcon[3, 3] = -0.91783782;
            nthcon[3, 4] = 0.0; // nthcon(3, 4) = 0#
            nthcon[3, 5] = 0.0; // nthcon(3, 5) = 0#

            // i = 4
            nthcon[4, 0] = -1.8525999;
            nthcon[4, 1] = 0.9340469;
            nthcon[4, 2] = 0.0;
            nthcon[4, 3] = 0.0;
            nthcon[4, 4] = 0.0;
            nthcon[4, 5] = 0.0;
        }

        public void InitFieldsreg1()
        {
            // 
            // Initialize coefficients and exponents for region 1
            // 
            ireg1[1] = 0;
            ireg1[2] = 0;
            ireg1[3] = 0;
            ireg1[4] = 0;
            ireg1[5] = 0;
            ireg1[6] = 0;
            ireg1[7] = 0;
            ireg1[8] = 0;
            ireg1[9] = 1;
            ireg1[10] = 1;
            ireg1[11] = 1;
            ireg1[12] = 1;
            ireg1[13] = 1;
            ireg1[14] = 1;
            ireg1[15] = 2;
            ireg1[16] = 2;
            ireg1[17] = 2;
            ireg1[18] = 2;
            ireg1[19] = 2;
            ireg1[20] = 3;
            ireg1[21] = 3;
            ireg1[22] = 3;
            ireg1[23] = 4;
            ireg1[24] = 4;
            ireg1[25] = 4;
            ireg1[26] = 5;
            ireg1[27] = 8;
            ireg1[28] = 8;
            ireg1[29] = 21;
            ireg1[30] = 23;
            ireg1[31] = 29;
            ireg1[32] = 30;
            ireg1[33] = 31;
            ireg1[34] = 32;
            // 
            jreg1[1] = -2;
            jreg1[2] = -1;
            jreg1[3] = 0;
            jreg1[4] = 1;
            jreg1[5] = 2;
            jreg1[6] = 3;
            jreg1[7] = 4;
            jreg1[8] = 5;
            jreg1[9] = -9;
            jreg1[10] = -7;
            jreg1[11] = -1;
            jreg1[12] = 0;
            jreg1[13] = 1;
            jreg1[14] = 3;
            jreg1[15] = -3;
            jreg1[16] = 0;
            jreg1[17] = 1;
            jreg1[18] = 3;
            jreg1[19] = 17;
            jreg1[20] = -4;
            jreg1[21] = 0;
            jreg1[22] = 6;
            jreg1[23] = -5;
            jreg1[24] = -2;
            jreg1[25] = 10;
            jreg1[26] = -8;
            jreg1[27] = -11;
            jreg1[28] = -6;
            jreg1[29] = -29;
            jreg1[30] = -31;
            jreg1[31] = -38;
            jreg1[32] = -39;
            jreg1[33] = -40;
            jreg1[34] = -41;
            // 
            nreg1[1] = 0.14632971213167d;
            nreg1[2] = -0.84548187169114d;
            nreg1[3] = -3.756360367204d;
            nreg1[4] = 3.3855169168385d;
            nreg1[5] = -0.95791963387872d;
            nreg1[6] = 0.15772038513228d;
            nreg1[7] = -0.016616417199501d;
            nreg1[8] = 8.1214629983568E-04d;
            nreg1[9] = 2.8319080123804E-04d;
            nreg1[10] = -6.0706301565874E-04d;
            nreg1[11] = -0.018990068218419d;
            nreg1[12] = -0.032529748770505d;
            nreg1[13] = -0.021841717175414d;
            nreg1[14] = -5.283835796993E-05d;
            nreg1[15] = -4.7184321073267E-04d;
            nreg1[16] = -3.0001780793026E-04d;
            nreg1[17] = 4.7661393906987E-05d;
            nreg1[18] = -4.4141845330846E-06d;
            nreg1[19] = -7.2694996297594E-16d;
            nreg1[20] = -3.1679644845054E-05d;
            nreg1[21] = -2.8270797985312E-06d;
            nreg1[22] = -8.5205128120103E-10d;
            nreg1[23] = -2.2425281908E-06d;
            nreg1[24] = -6.5171222895601E-07d;
            nreg1[25] = -1.4341729937924E-13d;
            nreg1[26] = -4.0516996860117E-07d;
            nreg1[27] = -1.2734301741641E-09d;
            nreg1[28] = -1.7424871230634E-10d;
            nreg1[29] = -6.8762131295531E-19d;
            nreg1[30] = 1.4478307828521E-20d;
            nreg1[31] = 2.6335781662795E-23d;
            nreg1[32] = -1.1947622640071E-23d;
            nreg1[33] = 1.8228094581404E-24d;
            nreg1[34] = -9.3537087292458E-26d;
            // 
        }
        // 
        // 
        // 
        public void InitFieldsreg2()
        {
            // 
            // Initialize coefficients and exponents for region 2
            // 
            j0reg2[1] = 0;
            j0reg2[2] = 1;
            j0reg2[3] = -5;
            j0reg2[4] = -4;
            j0reg2[5] = -3;
            j0reg2[6] = -2;
            j0reg2[7] = -1;
            j0reg2[8] = 2;
            j0reg2[9] = 3;
            // 
            n0reg2[1] = -9.6927686500217d;
            n0reg2[2] = 10.086655968018d;
            n0reg2[3] = -0.005608791128302d;
            n0reg2[4] = 0.071452738081455d;
            n0reg2[5] = -0.40710498223928d;
            n0reg2[6] = 1.4240819171444d;
            n0reg2[7] = -4.383951131945d;
            n0reg2[8] = -0.28408632460772d;
            n0reg2[9] = 0.021268463753307d;
            // 
            ireg2[1] = 1;
            ireg2[2] = 1;
            ireg2[3] = 1;
            ireg2[4] = 1;
            ireg2[5] = 1;
            ireg2[6] = 2;
            ireg2[7] = 2;
            ireg2[8] = 2;
            ireg2[9] = 2;
            ireg2[10] = 2;
            ireg2[11] = 3;
            ireg2[12] = 3;
            ireg2[13] = 3;
            ireg2[14] = 3;
            ireg2[15] = 3;
            ireg2[16] = 4;
            ireg2[17] = 4;
            ireg2[18] = 4;
            ireg2[19] = 5;
            ireg2[20] = 6;
            ireg2[21] = 6;
            ireg2[22] = 6;
            ireg2[23] = 7;
            ireg2[24] = 7;
            ireg2[25] = 7;
            ireg2[26] = 8;
            ireg2[27] = 8;
            ireg2[28] = 9;
            ireg2[29] = 10;
            ireg2[30] = 10;
            ireg2[31] = 10;
            ireg2[32] = 16;
            ireg2[33] = 16;
            ireg2[34] = 18;
            ireg2[35] = 20;
            ireg2[36] = 20;
            ireg2[37] = 20;
            ireg2[38] = 21;
            ireg2[39] = 22;
            ireg2[40] = 23;
            ireg2[41] = 24;
            ireg2[42] = 24;
            ireg2[43] = 24;
            // 
            jreg2[1] = 0;
            jreg2[2] = 1;
            jreg2[3] = 2;
            jreg2[4] = 3;
            jreg2[5] = 6;
            jreg2[6] = 1;
            jreg2[7] = 2;
            jreg2[8] = 4;
            jreg2[9] = 7;
            jreg2[10] = 36;
            jreg2[11] = 0;
            jreg2[12] = 1;
            jreg2[13] = 3;
            jreg2[14] = 6;
            jreg2[15] = 35;
            jreg2[16] = 1;
            jreg2[17] = 2;
            jreg2[18] = 3;
            jreg2[19] = 7;
            jreg2[20] = 3;
            jreg2[21] = 16;
            jreg2[22] = 35;
            jreg2[23] = 0;
            jreg2[24] = 11;
            jreg2[25] = 25;
            jreg2[26] = 8;
            jreg2[27] = 36;
            jreg2[28] = 13;
            jreg2[29] = 4;
            jreg2[30] = 10;
            jreg2[31] = 14;
            jreg2[32] = 29;
            jreg2[33] = 50;
            jreg2[34] = 57;
            jreg2[35] = 20;
            jreg2[36] = 35;
            jreg2[37] = 48;
            jreg2[38] = 21;
            jreg2[39] = 53;
            jreg2[40] = 39;
            jreg2[41] = 26;
            jreg2[42] = 40;
            jreg2[43] = 58;
            // 
            nreg2[1] = -1.7731742473213E-03d;
            nreg2[2] = -0.017834862292358d;
            nreg2[3] = -0.045996013696365d;
            nreg2[4] = -0.057581259083432d;
            nreg2[5] = -0.05032527872793d;
            nreg2[6] = -3.3032641670203E-05d;
            nreg2[7] = -1.8948987516315E-04d;
            nreg2[8] = -3.9392777243355E-03d;
            nreg2[9] = -0.043797295650573d;
            nreg2[10] = -2.6674547914087E-05d;
            nreg2[11] = 2.0481737692309E-08d;
            nreg2[12] = 4.3870667284435E-07d;
            nreg2[13] = -3.227767723857E-05d;
            nreg2[14] = -1.5033924542148E-03d;
            nreg2[15] = -0.040668253562649d;
            nreg2[16] = -7.8847309559367E-10d;
            nreg2[17] = 1.2790717852285E-08d;
            nreg2[18] = 4.8225372718507E-07d;
            nreg2[19] = 2.2922076337661E-06d;
            nreg2[20] = -1.6714766451061E-11d;
            nreg2[21] = -2.1171472321355E-03d;
            nreg2[22] = -23.895741934104d;
            nreg2[23] = -5.905956432427E-18d;
            nreg2[24] = -1.2621808899101E-06d;
            nreg2[25] = -0.038946842435739d;
            nreg2[26] = 1.1256211360459E-11d;
            nreg2[27] = -8.2311340897998d;
            nreg2[28] = 1.9809712802088E-08d;
            nreg2[29] = 1.0406965210174E-19d;
            nreg2[30] = -1.0234747095929E-13d;
            nreg2[31] = -1.0018179379511E-09d;
            nreg2[32] = -8.0882908646985E-11d;
            nreg2[33] = 0.10693031879409d;
            nreg2[34] = -0.33662250574171d;
            nreg2[35] = 8.9185845355421E-25d;
            nreg2[36] = 3.0629316876232E-13d;
            nreg2[37] = -4.2002467698208E-06d;
            nreg2[38] = -5.9056029685639E-26d;
            nreg2[39] = 3.7826947613457E-06d;
            nreg2[40] = -1.2768608934681E-15d;
            nreg2[41] = 7.3087610595061E-29d;
            nreg2[42] = 5.5414715350778E-17d;
            nreg2[43] = -9.436970724121E-07d;
            // 
        }
        // 
        // 
        // 
        public void InitFieldsreg3()
        {
            // 
            // Initialize coefficients and exponents for region 3
            // 
            ireg3[1] = 0;
            ireg3[2] = 0;
            ireg3[3] = 0;
            ireg3[4] = 0;
            ireg3[5] = 0;
            ireg3[6] = 0;
            ireg3[7] = 0;
            ireg3[8] = 0;
            ireg3[9] = 1;
            ireg3[10] = 1;
            ireg3[11] = 1;
            ireg3[12] = 1;
            ireg3[13] = 2;
            ireg3[14] = 2;
            ireg3[15] = 2;
            ireg3[16] = 2;
            ireg3[17] = 2;
            ireg3[18] = 2;
            ireg3[19] = 3;
            ireg3[20] = 3;
            ireg3[21] = 3;
            ireg3[22] = 3;
            ireg3[23] = 3;
            ireg3[24] = 4;
            ireg3[25] = 4;
            ireg3[26] = 4;
            ireg3[27] = 4;
            ireg3[28] = 5;
            ireg3[29] = 5;
            ireg3[30] = 5;
            ireg3[31] = 6;
            ireg3[32] = 6;
            ireg3[33] = 6;
            ireg3[34] = 7;
            ireg3[35] = 8;
            ireg3[36] = 9;
            ireg3[37] = 9;
            ireg3[38] = 10;
            ireg3[39] = 10;
            ireg3[40] = 11;
            // 
            jreg3[1] = 0;
            jreg3[2] = 0;
            jreg3[3] = 1;
            jreg3[4] = 2;
            jreg3[5] = 7;
            jreg3[6] = 10;
            jreg3[7] = 12;
            jreg3[8] = 23;
            jreg3[9] = 2;
            jreg3[10] = 6;
            jreg3[11] = 15;
            jreg3[12] = 17;
            jreg3[13] = 0;
            jreg3[14] = 2;
            jreg3[15] = 6;
            jreg3[16] = 7;
            jreg3[17] = 22;
            jreg3[18] = 26;
            jreg3[19] = 0;
            jreg3[20] = 2;
            jreg3[21] = 4;
            jreg3[22] = 16;
            jreg3[23] = 26;
            jreg3[24] = 0;
            jreg3[25] = 2;
            jreg3[26] = 4;
            jreg3[27] = 26;
            jreg3[28] = 1;
            jreg3[29] = 3;
            jreg3[30] = 26;
            jreg3[31] = 0;
            jreg3[32] = 2;
            jreg3[33] = 26;
            jreg3[34] = 2;
            jreg3[35] = 26;
            jreg3[36] = 2;
            jreg3[37] = 26;
            jreg3[38] = 0;
            jreg3[39] = 1;
            jreg3[40] = 26;
            // 
            nreg3[1] = 1.0658070028513d;
            nreg3[2] = -15.732845290239d;
            nreg3[3] = 20.944396974307d;
            nreg3[4] = -7.6867707878716d;
            nreg3[5] = 2.6185947787954d;
            nreg3[6] = -2.808078114862d;
            nreg3[7] = 1.2053369696517d;
            nreg3[8] = -8.4566812812502E-03d;
            nreg3[9] = -1.2654315477714d;
            nreg3[10] = -1.1524407806681d;
            nreg3[11] = 0.88521043984318d;
            nreg3[12] = -0.64207765181607d;
            nreg3[13] = 0.38493460186671d;
            nreg3[14] = -0.85214708824206d;
            nreg3[15] = 4.8972281541877d;
            nreg3[16] = -3.0502617256965d;
            nreg3[17] = 0.039420536879154d;
            nreg3[18] = 0.12558408424308d;
            nreg3[19] = -0.2799932969871d;
            nreg3[20] = 1.389979956946d;
            nreg3[21] = -2.018991502357d;
            nreg3[22] = -8.2147637173963E-03d;
            nreg3[23] = -0.47596035734923d;
            nreg3[24] = 0.0439840744735d;
            nreg3[25] = -0.44476435428739d;
            nreg3[26] = 0.90572070719733d;
            nreg3[27] = 0.70522450087967d;
            nreg3[28] = 0.10770512626332d;
            nreg3[29] = -0.32913623258954d;
            nreg3[30] = -0.50871062041158d;
            nreg3[31] = -0.022175400873096d;
            nreg3[32] = 0.094260751665092d;
            nreg3[33] = 0.16436278447961d;
            nreg3[34] = -0.013503372241348d;
            nreg3[35] = -0.014834345352472d;
            nreg3[36] = 5.7922953628084E-04d;
            nreg3[37] = 3.2308904703711E-03d;
            nreg3[38] = 8.0964802996215E-05d;
            nreg3[39] = -1.6557679795037E-04d;
            nreg3[40] = -4.4923899061815E-05d;
            // 
        }
        // 
        // 
        // 
        public void InitFieldsreg4()
        {
            // 
            // Initialize coefficients for region 4
            // 
            nreg4[1] = 1167.0521452767d;
            nreg4[2] = -724213.16703206d;
            nreg4[3] = -17.073846940092d;
            nreg4[4] = 12020.82470247d;
            nreg4[5] = -3232555.0322333d;
            nreg4[6] = 14.91510861353d;
            nreg4[7] = -4823.2657361591d;
            nreg4[8] = 405113.40542057d;
            nreg4[9] = -0.23855557567849d;
            nreg4[10] = 650.17534844798d;
            // 
        }

        private void InitFieldsbound()
        {
            nbound[1] = 348.05185628969; nbound[2] = -1.1671859879975; nbound[3] = 0.0010192970039326;
            nbound[4] = 572.54459862746; nbound[5] = 13.91883977887;
        }

        #endregion
        private double GammaReg1(double tau, double pi)
        {
            // Fundamental equation for region 1
            double result = 0.0;
            InitFieldsreg1();

            for (int i = 1; i <= 34; i++)
            {
                result += nreg1[i] * Math.Pow(7.1 - pi, ireg1[i]) * Math.Pow(tau - 1.222, jreg1[i]);
            }

            return result;
        }
        private double GammaPiReg1(double tau, double pi)
        {
            // First derivative of fundamental equation in pi for region 1
            double result = 0.0;
            InitFieldsreg1();

            for (int i = 1; i <= 34; i++)
            {
                result -= nreg1[i] * ireg1[i] * Math.Pow(7.1 - pi, ireg1[i] - 1) * Math.Pow(tau - 1.222, jreg1[i]);
            }

            return result;
        }
        private double GammaPiPiReg1(double tau, double pi)
        {
            // Second derivative of fundamental equation in pi for region 1
            double result = 0.0;
            InitFieldsreg1();

            for (int i = 1; i <= 34; i++)
            {
                result += nreg1[i] * ireg1[i] * (ireg1[i] - 1) * Math.Pow(7.1 - pi, ireg1[i] - 2) * Math.Pow(tau - 1.222, jreg1[i]);
            }

            return result;
        }
        private double GammaTauReg1(double tau, double pi)
        {
            // First derivative of fundamental equation in tau for region 1
            double result = 0.0;
            InitFieldsreg1();

            for (int i = 1; i <= 34; i++)
            {
                result += nreg1[i] * Math.Pow(7.1 - pi, ireg1[i]) * jreg1[i] * Math.Pow(tau - 1.222, jreg1[i] - 1);
            }

            return result;
        }
        private double GammaTauTauReg1(double tau, double pi)
        {
            // Second derivative of fundamental equation in tau for region 1
            double result = 0.0;
            InitFieldsreg1();

            for (int i = 1; i <= 34; i++)
            {
                result += nreg1[i] * Math.Pow(7.1 - pi, ireg1[i]) * jreg1[i] * (jreg1[i] - 1) * Math.Pow(tau - 1.222, jreg1[i] - 2);
            }

            return result;
        }
        private double GammaPiTauReg1(double tau, double pi)
        {
            // Second derivative of fundamental equation in pi and tau for region 1
            double result = 0.0;
            InitFieldsreg1();

            for (int i = 1; i <= 34; i++)
            {
                result -= nreg1[i] * ireg1[i] * Math.Pow(7.1 - pi, ireg1[i] - 1) *
                          jreg1[i] * Math.Pow(tau - 1.222, jreg1[i] - 1);
            }

            return result;
        }
        private double Gamma0Reg2(double tau, double pi)
        {
            // Ideal-gas part of fundamental equation for region 2
            double result = Math.Log(pi);
            InitFieldsreg2();

            for (int i = 1; i <= 9; i++)
            {
                result += nreg2[i] * Math.Pow(tau, jreg2[i]);
            }

            return result;
        }
        private double Gamma0PiReg2(double tau, double pi)
        {
            // First derivative in pi of ideal-gas part of fundamental equation for region 2
            return 1.0 / pi;
        }
        private double Gamma0PiPiReg2(double tau, double pi)
        {
            // Second derivative in pi of ideal-gas part of fundamental equation for region 2
            return -1.0 / (pi * pi);
        }
        private double Gamma0TauReg2(double tau, double pi)
        {
            // First derivative in tau of ideal-gas part of fundamental equation for region 2
            // Returns gamma0taureg2 in VB.NET

            double result = 0.0;
            InitFieldsreg2(); // Asegúrate de que esta inicialización ya esté implementada

            for (int i = 1; i <= 9; i++)
            {
                result += n0reg2[i] * j0reg2[i] * Math.Pow(tau, j0reg2[i] - 1);
            }

            return result;
        }
        private double Gamma0TauTauReg2(double tau, double pi)
        {
            // Second derivative in tau of ideal-gas part of fundamental equation for region 2
            double result = 0.0;
            InitFieldsreg2();

            for (int i = 1; i <= 9; i++)
            {
                result += nreg2[i] * jreg2[i] * (jreg2[i] - 1) * Math.Pow(tau, jreg2[i] - 2);
            }

            return result;
        }
        private double Gamma0PiTauReg2(double tau, double pi)
        {
            // Second derivative in pi and tau of ideal-gas part for region 2
            return 0;
        }
        private double GammaRReg2(double tau, double pi)
        {
            // Residual part of fundamental equation for region 2
            double result = 0.0;
            InitFieldsreg2();

            for (int i = 1; i <= 43; i++)
            {
                result += nreg2[i] * Math.Pow(pi, ireg2[i]) * Math.Pow(tau - 0.5, jreg2[i]);
            }

            return result;
        }
        private double GammaRPiReg2(double tau, double pi)
        {
            // First derivative in pi of residual part for region 2
            double result = 0.0;
            InitFieldsreg2();

            for (int i = 1; i <= 43; i++)
            {
                result += nreg2[i] * ireg2[i] * Math.Pow(pi, ireg2[i] - 1) * Math.Pow(tau - 0.5, jreg2[i]);
            }

            return result;
        }
        private double GammaRPiPiReg2(double tau, double pi)
        {
            // Second derivative in pi of residual part for region 2
            double result = 0.0;
            InitFieldsreg2();

            for (int i = 1; i <= 43; i++)
            {
                result += nreg2[i] * ireg2[i] * (ireg2[i] - 1) * Math.Pow(pi, ireg2[i] - 2) * Math.Pow(tau - 0.5, jreg2[i]);
            }

            return result;
        }
        private double GammaRTauReg2(double tau, double pi)
        {
            // First derivative in tau of residual part of fundamental equation for region 2
            // Returns gammartaureg2 in VB.NET

            double result = 0.0;
            InitFieldsreg2(); // Asegúrate de tener esta inicialización implementada

            for (int i = 1; i <= 43; i++)
            {
                result += nreg2[i] * Math.Pow(pi, ireg2[i]) * jreg2[i] * Math.Pow(tau - 0.5, jreg2[i] - 1);
            }

            return result;
        }
        private double GammaRTauTauReg2(double tau, double pi)
        {
            // Second derivative in tau of residual part for region 2
            double result = 0.0;
            InitFieldsreg2();

            for (int i = 1; i <= 43; i++)
            {
                result += nreg2[i] * Math.Pow(pi, ireg2[i]) * jreg2[i] * (jreg2[i] - 1) * Math.Pow(tau - 0.5, jreg2[i] - 2);
            }

            return result;
        }
        private double GammaRPiTauReg2(double tau, double pi)
        {
            // Second derivative in pi and tau of residual part for region 2
            double result = 0.0;
            InitFieldsreg2();

            for (int i = 1; i <= 43; i++)
            {
                result += nreg2[i] * ireg2[i] * Math.Pow(pi, ireg2[i] - 1) *
                          jreg2[i] * Math.Pow(tau - 0.5, jreg2[i] - 1);
            }

            return result;
        }
        private double Fireg3(double tau, double delta)
        {
            // Fundamental equation for region 3
            double result = nreg3[1] * Math.Log(delta);
            InitFieldsreg3();

            for (int i = 2; i <= 40; i++)
            {
                result += nreg3[i] * Math.Pow(delta, ireg3[i]) * Math.Pow(tau, jreg3[i]);
            }

            return result;
        }
        private double Fideltareg3(double tau, double delta)
        {
            // First derivative in delta of fundamental equation for region 3
            double result = nreg3[1] / delta;
            InitFieldsreg3();

            for (int i = 2; i <= 40; i++)
            {
                result += nreg3[i] * ireg3[i] * Math.Pow(delta, ireg3[i] - 1) * Math.Pow(tau, jreg3[i]);
            }

            return result;
        }
        private double Fideltadeltareg3(double tau, double delta)
        {
            // Second derivative in delta of fundamental equation for region 3
            double result = -nreg3[1] / (delta * delta);
            InitFieldsreg3();

            for (int i = 2; i <= 40; i++)
            {
                result += nreg3[i] * ireg3[i] * (ireg3[i] - 1) * Math.Pow(delta, ireg3[i] - 2) * Math.Pow(tau, jreg3[i]);
            }

            return result;
        }
        private double Fideltataureg3(double tau, double delta)
        {
            // Second derivative in delta and tau of fundamental equation for region 3
            double result = 0;
            InitFieldsreg3();

            for (int i = 2; i <= 40; i++)
            {
                result += nreg3[i] * ireg3[i] * Math.Pow(delta, ireg3[i] - 1) *
                          jreg3[i] * Math.Pow(tau, jreg3[i] - 1);
            }

            return result;
        }
        private double PsiVisc(double tau, double delta)
        {
            // Reduced dynamic viscosity
            double psi0 = 0;
            double psi1 = 0;

            InitFieldsvisc();

            for (int i = 0; i <= 3; i++)
            {
                psi0 += n0visc[i] * Math.Pow(tau, i);
            }

            psi0 = 1 / (Math.Pow(tau, 0.5) * psi0);

            for (int i = 1; i <= 19; i++)
            {
                psi1 += nvisc[i] * Math.Pow(delta - 1.0, ivisc[i]) * Math.Pow(tau - 1.0, jvisc[i]);
            }

            psi1 = Math.Exp(delta * psi1);

            return psi0 * psi1;
        }
        private double LambThCon(double temperature, double pressure, double tau, double delta)
        {
            // Reduced thermal conductivity
            double lamb0 = 0;
            double lamb1 = 0;
            double dpidtau = 0;
            double ddeltadpi = 0;

            InitFieldsThCon();

            // lambda0: base term
            for (int i = 0; i <= 3; i++)
            {
                lamb0 += n0thcon[i] * Math.Pow(tau, i);
            }

            lamb0 = 1.0 / (Math.Pow(tau, 0.5) * lamb0);

            // lambda1: polynomial + exponential
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 5; j++)
                {
                    lamb1 += nthcon[i, j] * Math.Pow(tau - 1.0, i) * Math.Pow(delta - 1.0, j);
                }
            }

            lamb1 = Math.Exp(delta * lamb1);

            // Lambda2: correction term
            double lamb2 = 0;

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1
                double taus = 1386.0 / temperature;
                double pis = pressure / 165.3;

                double gammapitau = GammaPiTauReg1(taus, pis);
                double gammapi = GammaPiReg1(taus, pis);
                double gammapipip = GammaPiPiReg1(taus, pis);

                dpidtau = (647.226 * 165.3 * (gammapitau * 1386.0 - gammapi * temperature)) /
                          (221.15 * Math.Pow(temperature, 2) * gammapipip);
                ddeltadpi = -(22115000.0 * gammapipip) / (317.763 * rgas_water * temperature * Math.Pow(gammapi, 2));
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2
                double taus = 540.0 / temperature;
                double pis = pressure / 10.0;

                double gamma0pitaureg2 = Gamma0PiTauReg2(taus, pis);
                double gammarpitaureg2 = GammaRPiTauReg2(taus, pis);
                double gamma0pireg2 = Gamma0PiReg2(taus, pis);
                double gammarpireg2 = GammaRPiReg2(taus, pis);
                double gammapipireg2 = GammaRPiPiReg2(taus, pis);

                dpidtau = (647.226 * 10.0 * ((gamma0pitaureg2 + gammarpitaureg2) * 540.0 - (gamma0pireg2 + gammarpireg2) * temperature)) /
                          (221.15 * Math.Pow(temperature, 2) * (Gamma0PiPiReg2(taus, pis) + gammapipireg2));
                ddeltadpi = -(22115000.0 * (Gamma0PiPiReg2(taus, pis) + gammapipireg2)) /
                            (317.763 * rgas_water * temperature * Math.Pow(gamma0pireg2 + gammarpireg2, 2));
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) && pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3
                double taus = 647.096 / temperature;
                double deltas = delta * 317.763 / 322.0;

                double fideltareg3 = Fideltareg3(taus, deltas);
                double fideltataureg3 = Fideltataureg3(taus, deltas);
                double fideltadeltareg3 = Fideltadeltareg3(taus, deltas);

                dpidtau = (647.226 * rgas_water * Math.Pow(delta * 317.763, 2) *
                           (fideltareg3 - taus * fideltataureg3)) / (22115000.0 * 322.0);

                ddeltadpi = (22115000.0 * 322.0) /
                            (317.763 * delta * 317.763 * rgas_water * temperature *
                             (2 * fideltareg3 + delta * 317.763 / 322.0 * fideltadeltareg3));
            }
            else
            {
                dpidtau = 0;
                ddeltadpi = 0;
            }

            // Lambda2 calculation
            double psiVisc = PsiVisc(tau, delta);
            lamb2 = 0.0013848 / psiVisc * Math.Pow(tau * delta, -2) *
                    Math.Pow(dpidtau, 2) * Math.Pow(delta * ddeltadpi, 0.4678) *
                    Math.Pow(delta, 0.5) * Math.Exp(-18.66 * Math.Pow(1.0 / tau - 1.0, 2) - Math.Pow(delta - 1.0, 4));

            // Final result
            return lamb0 * lamb1 + lamb2;
        }
        public double PSatW(double temperature)
        {
            // Saturation pressure of water
            // Returns pressure in bar
            // temperature in K
            // If temperature is out of range, returns -1

            if (temperature < 273.15 || temperature > 647.096)
                return -1.0;

            InitFieldsreg4();

            double del = temperature + nreg4[9] / (temperature - nreg4[10]);
            double aco = Math.Pow(del, 2) + nreg4[1] * del + nreg4[2];
            double bco = nreg4[3] * Math.Pow(del, 2) + nreg4[4] * del + nreg4[5];
            double cco = nreg4[6] * Math.Pow(del, 2) + nreg4[7] * del + nreg4[8];

            double discriminant = Math.Sqrt(bco * bco - 4 * aco * cco);
            double denominator = -bco + discriminant;
            double result = Math.Pow(2 * cco / denominator, 4) * 10;

            return result;
        }
        public double TSatW(double pressure)
        {
            // Saturation temperature of water
            // Returns temperature in K
            // pressure in bar
            // If pressure is out of range, returns -1

            if (pressure < 0.00611213 || pressure > 220.64)
                return -1.0;

            InitFieldsreg4();

            double bet = Math.Pow(0.1 * pressure, 0.25);
            double eco = Math.Pow(bet, 2) + nreg4[3] * bet + nreg4[6];
            double fco = nreg4[1] * Math.Pow(bet, 2) + nreg4[4] * bet + nreg4[7];
            double gco = nreg4[2] * Math.Pow(bet, 2) + nreg4[5] * bet + nreg4[8];

            double sqrtTerm = Math.Sqrt(fco * fco - 4 * eco * gco);
            double dco = 2 * gco / (-fco - sqrtTerm);

            double innerSqrt = Math.Sqrt((nreg4[10] + dco) * (nreg4[10] + dco) - 4 * (nreg4[9] + nreg4[10] * dco));
            double tSat = 0.5 * (nreg4[10] + dco - innerSqrt);

            return tSat;
        }
        private double pBound(double temperature)
        {
            // Boundary pressure between regions 2 and 3
            // Returns pressure in bar
            // temperature in K
            // If temperature is out of range, returns -1

            if (temperature < 623.15 || temperature > 863.15)
                return -1.0;

            InitFieldsbound();

            return (nbound[1] + nbound[2] * temperature + nbound[3] * Math.Pow(temperature, 2)) * 10.0;
        }
        private double tBound(double pressure)
        {
            // Boundary temperature between regions 2 and 3
            // Returns temperature in K
            // pressure in bar
            // If pressure is out of range, returns -1

            if (pressure < 165.292 || pressure > 1000.0)
                return -1.0;

            InitFieldsbound();

            double term = (0.1 * pressure - nbound[5]) / nbound[3];
            if (term < 0)
                return -1.0; // Avoid negative sqrt

            return nbound[4] + Math.Sqrt(term);
        }
        private double VolReg1(double temperature, double pressure)
        {
            // Specific volume in region 1
            // Returns volreg1 in m^3/kg
            double tau = 1386.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaPi = GammaPiReg1(tau, pi);
            return rgas_water * temperature * pi * gammaPi / (pressure * 100000.0);
        }
        private double EnergyReg1(double temperature, double pressure)
        {
            // Specific internal energy in region 1
            // Returns energyreg1 in kJ/kg
            double tau = 1386.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaTau = GammaTauReg1(tau, pi);
            double gammaPi = GammaPiReg1(tau, pi);

            return 0.001 * rgas_water * temperature * (tau * gammaTau - pi * gammaPi);
        }
        private double EntropyReg1(double temperature, double pressure)
        {
            // Specific entropy in region 1
            // Returns entropyreg1 in kJ/(kg·K)
            double tau = 1386.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaTau = GammaTauReg1(tau, pi);
            double gammareg1 = GammaReg1(tau, pi);

            return 0.001 * rgas_water * (tau * gammaTau - gammareg1);
        }
        private double EnthalpyReg1(double temperature, double pressure)
        {
            // Specific enthalpy in region 1
            // Returns enthalpyreg1 in kJ/kg
            double tau = 1386.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaTau = GammaTauReg1(tau, pi);
            return 0.001 * rgas_water * temperature * tau * gammaTau;
        }
        private double CpReg1(double temperature, double pressure)
        {
            // Specific isobaric heat capacity in region 1
            // Returns cpreg1 in kJ/(kg·K)
            double tau = 1386.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaTauTau = GammaTauTauReg1(tau, pi);
            return -0.001 * rgas_water * Math.Pow(tau, 2) * gammaTauTau;
        }
        private double CvReg1(double temperature, double pressure)
        {
            // Specific isochoric heat capacity in region 1
            // Returns cvreg1 in kJ/(kg·K)
            double tau = 1386.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaTauTau = GammaTauTauReg1(tau, pi);
            double gammaPi = GammaPiReg1(tau, pi);
            double gammaPiTau = GammaPiTauReg1(tau, pi);
            double gammaPiPi = GammaPiPiReg1(tau, pi);

            return 0.001 * rgas_water * (-Math.Pow(tau, 2) * gammaTauTau +
                Math.Pow(gammaPi - tau * gammaPiTau, 2) / gammaPiPi);
        }
        private double VolReg2(double temperature, double pressure)
        {
            // Specific volume in region 2
            // Returns volreg2 in m³/kg
            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gamma0Pi = Gamma0PiReg2(tau, pi);
            double gammaRPi = GammaRPiReg2(tau, pi);

            return rgas_water * temperature * pi * (gamma0Pi + gammaRPi) / (pressure * 100000.0);
        }
        private double EnergyReg2(double temperature, double pressure)
        {
            // Specific internal energy in region 2
            // Returns energyreg2 in kJ/kg
            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gamma0Tau = Gamma0TauReg2(tau, pi);
            double gammaRTau = GammaRTauReg2(tau, pi);
            double gamma0Pi = Gamma0PiReg2(tau, pi);
            double gammaRPi = GammaRPiReg2(tau, pi);

            return 0.001 * rgas_water * temperature *
                (tau * (gamma0Tau + gammaRTau) - pi * (gamma0Pi + gammaRPi));
        }
        private double EntropyReg2(double temperature, double pressure)
        {
            // Specific entropy in region 2
            // Returns entropyreg2 in kJ/(kg·K)
            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gamma0 = Gamma0Reg2(tau, pi);
            double gammar = GammaRReg2(tau, pi);
            double gamma0Tau = Gamma0TauReg2(tau, pi);
            double gammaRTau = GammaRTauReg2(tau, pi);

            return 0.001 * rgas_water *
                (tau * (gamma0Tau + gammaRTau) - (gamma0 + gammar));
        }
        private double EnthalpyReg2(double temperature, double pressure)
        {
            // Specific enthalpy in region 2
            // Returns enthalpyreg2 in kJ/kg
            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gamma0Tau = Gamma0TauReg2(tau, pi);
            double gammaRTau = GammaRTauReg2(tau, pi);

            return 0.001 * rgas_water * temperature * tau * (gamma0Tau + gammaRTau);
        }
        private double CpReg2(double temperature, double pressure)
        {
            // Specific isobaric heat capacity in region 2
            // Returns cpreg2 in kJ/(kg·K)
            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gamma0TauTau = Gamma0TauTauReg2(tau, pi);
            double gammaRTauTau = GammaRTauTauReg2(tau, pi);

            return -0.001 * rgas_water * Math.Pow(tau, 2) * (gamma0TauTau + gammaRTauTau);
        }
        private double CvReg2(double temperature, double pressure)
        {
            // Specific isochoric heat capacity in region 2
            // Returns cvreg2 in kJ/(kg·K)
            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gammaRPi = GammaRPiReg2(tau, pi);
            double gammaRPiTau = GammaRPiTauReg2(tau, pi);
            double gammaRPiPi = GammaRPiPiReg2(tau, pi);

            double numerator = Math.Pow(1 + pi * gammaRPi - tau * pi * gammaRPiTau, 2);
            double denominator = 1 - Math.Pow(pi, 2) * gammaRPiPi;

            return 0.001 * rgas_water * (-Math.Pow(tau, 2) * (Gamma0TauTauReg2(tau, pi) + GammaRTauTauReg2(tau, pi)) - numerator / denominator);
        }
        private double PressReg3(double temperature, double density)
        {
            // Pressure in region 3
            // Returns pressure in bar
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            return density * rgas_water * temperature * delta * Fideltareg3(tau, delta) / 100000.0;
        }
        private double Fitaureg3(double tau, double delta)
        {
            // First derivative in tau of fundamental equation for region 3
            // Returns: fitaureg3 = ∂f/∂τ for region 3

            double result = 0;
            InitFieldsreg3(); // Asegúrate de que esta inicialización ya esté implementada

            for (int i = 2; i <= 40; i++)
            {
                result += nreg3[i] * Math.Pow(delta, ireg3[i]) * jreg3[i] * Math.Pow(tau, jreg3[i] - 1);
            }

            return result;
        }
        private double EnergyReg3(double temperature, double density)
        {
            // Specific internal energy in region 3
            // Returns energyreg3 in kJ/kg
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            return 0.001 * rgas_water * temperature * tau * Fitaureg3(tau, delta);
        }
        private double EntropyReg3(double temperature, double density)
        {
            // Specific entropy in region 3
            // Returns entropyreg3 in kJ/(kg·K)
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            return 0.001 * rgas_water * (tau * Fitaureg3(tau, delta) - Fireg3(tau, delta));
        }
        private double EnthalpyReg3(double temperature, double density)
        {
            // Specific enthalpy in region 3
            // Returns enthalpyreg3 in kJ/kg
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            return 0.001 * rgas_water * temperature *
                (tau * Fitaureg3(tau, delta) + delta * Fideltareg3(tau, delta));
        }
        private double Fitautaureg3(double tau, double delta)
        {
            // Second derivative in tau of fundamental equation for region 3

            double result = 0;
            InitFieldsreg3(); // Asegúrate de que esta inicialización ya esté implementada

            for (int i = 2; i <= 40; i++)
            {
                result += nreg3[i] * Math.Pow(delta, ireg3[i]) *
                          jreg3[i] * (jreg3[i] - 1) *
                          Math.Pow(tau, jreg3[i] - 2);
            }

            return result;
        }
        private double CpReg3(double temperature, double density)
        {
            // Specific isobaric heat capacity in region 3
            // Returns cpreg3 in kJ/(kg·K)
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            double numerator = Math.Pow(delta * Fideltareg3(tau, delta) - delta * tau * Fideltataureg3(tau, delta), 2);
            double denominator = 2 * delta * Fideltareg3(tau, delta) + Math.Pow(delta, 2) * Fideltadeltareg3(tau, delta);

            return 0.001 * rgas_water * (-Math.Pow(tau, 2) * Fitautaureg3(tau, delta) + numerator / denominator);
        }
        private double CvReg3(double temperature, double density)
        {
            // Specific isochoric heat capacity in region 3
            // Returns cvreg3 in kJ/(kg·K)
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            return 0.001 * rgas_water * (-Math.Pow(tau, 2) * Fitautaureg3(tau, delta));
        }
        private double DensReg3(double temperature, double pressure)
        {
            // Determine density in region 3 using Newton method
            // Returns densreg3 in kg/m³
            // temperature in K
            // pressure in bar
            // If not converged, returns -2

            double densold = (temperature < tc_water && pressure < PSatW(temperature)) ? 100.0 : 600.0;
            double tau = tc_water / temperature;

            for (int j = 1; j <= 1000; j++)
            {
                double delta = densold / dc_water;
                double derivprho = rgas_water * temperature / dc_water *
                    (2 * densold * Fideltareg3(tau, delta) + Math.Pow(densold, 2) / dc_water * Fideltadeltareg3(tau, delta));

                double pressureCalc = rgas_water * temperature * Math.Pow(densold, 2) / dc_water * Fideltareg3(tau, delta);
                double densnew = densold + (pressure * 100000.0 - pressureCalc) / derivprho;
                double diffdens = Math.Abs(densnew - densold);

                if (diffdens < 0.000005)
                {
                    return densnew;
                }

                densold = densnew;
            }

            return -2.0; // Not converged
        }
        public double DensW(double temperature, double pressure)
        {
            // Density of water or steam
            // Returns densW in kg/m³
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1 - Liquid compressed
                return 1 / VolReg1(temperature, pressure);
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2 - Vapor superheated
                return 1 / VolReg2(temperature, pressure);
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                     pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3 - Critical region
                return DensReg3(temperature, pressure);
            }
            else
            {
                // Outside valid range
                return -1;
            }
        }
        public double EnergyW(double temperature, double pressure)
        {
            // Specific internal energy of water or steam
            // Returns energyW in kJ/kg
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1 - Liquid compressed
                return EnergyReg1(temperature, pressure);
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2 - Vapor superheated
                return EnergyReg2(temperature, pressure);
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                     pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3 - Critical region
                double density = DensReg3(temperature, pressure);
                return EnergyReg3(temperature, density);
            }
            else
            {
                // Outside valid range
                return -1;
            }
        }
        public double EntropyW(double temperature, double pressure)
        {
            // Specific entropy of water or steam
            // Returns entropyW in kJ/(kg·K)
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1 - Liquid compressed
                return EntropyReg1(temperature, pressure);
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2 - Vapor superheated
                return EntropyReg2(temperature, pressure);
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                     pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3 - Critical region
                double density = DensReg3(temperature, pressure);
                return EntropyReg3(temperature, density);
            }
            else
            {
                // Outside valid range
                return -1;
            }
        }
        public double EnthalpyW(double temperature, double pressure)
        {
            // Specific enthalpy of water or steam
            // Returns enthalpyW in kJ/kg
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1 - Liquid compressed
                return EnthalpyReg1(temperature, pressure);
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2 - Vapor superheated
                return EnthalpyReg2(temperature, pressure);
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                     pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3 - Critical region
                double density = DensReg3(temperature, pressure);
                return EnthalpyReg3(temperature, density);
            }
            else
            {
                // Outside valid range
                return -1;
            }
        }
        public double CpW(double temperature, double pressure)
        {
            // Specific isobaric heat capacity of water or steam
            // Returns cpW in kJ/(kg·K)
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1 - Liquid compressed
                return CpReg1(temperature, pressure);
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2 - Vapor superheated
                return CpReg2(temperature, pressure);
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                     pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3 - Critical region
                double density = DensReg3(temperature, pressure);
                return CpReg3(temperature, density);
            }
            else
            {
                // Outside valid range
                return -1;
            }
        }
        public double CvW(double temperature, double pressure)
        {
            // Specific isochoric heat capacity of water or steam
            // Returns cvW in kJ/(kg·K)
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
            {
                // Region 1 - Liquid compressed
                return CvReg1(temperature, pressure);
            }
            else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                     (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                     (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
            {
                // Region 2 - Vapor superheated
                return CvReg2(temperature, pressure);
            }
            else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                     pressure >= pBound(temperature) && pressure <= 1000.0)
            {
                // Region 3 - Critical region
                double density = DensReg3(temperature, pressure);
                return CvReg3(temperature, density);
            }
            else
            {
                // Outside valid range
                return -1;
            }
        }
        private double SpsoundReg1(double temperature, double pressure)
        {
            // Speed of sound in region 1
            // Returns spsoundreg1 in m/s
            // temperature in K
            // pressure in bar

            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure / 16.53;

            double gammaPi = GammaPiReg1(tau, pi);
            double gammaPiTau = GammaPiTauReg1(tau, pi);
            double gammaTauTau = GammaTauTauReg1(tau, pi);
            double gammaPiPi = GammaPiPiReg1(tau, pi);

            double numerator = Math.Pow(gammaPi, 2);
            double denominator = ((Math.Pow(gammaPi - tau * gammaPiTau, 2) / (Math.Pow(tau, 2) * gammaTauTau)) - gammaPiPi);

            return Math.Sqrt(rgas_water * temperature * numerator / denominator);
        }
        private double SpsoundReg2(double temperature, double pressure)
        {
            // Speed of sound in region 2
            // Returns spsoundreg2 in m/s
            // temperature in K
            // pressure in bar

            double tau = 540.0 / temperature;
            double pi = 0.1 * pressure;

            double gammaRPi = GammaRPiReg2(tau, pi);
            double gammaRPiPi = GammaRPiReg2(tau, pi);
            double gammaRPiTau = GammaRPiReg2(tau, pi);

            double gamma0TauTau = Gamma0TauTauReg2(tau, pi);
            double gammaRTauTau = GammaRTauTauReg2(tau, pi);

            double numerator = 1 + 2 * pi * gammaRPi + Math.Pow(pi * gammaRPi, 2);
            double denominatorTerm1 = 1 - Math.Pow(pi, 2) * gammaRPiPi;
            double denominatorTerm2 = Math.Pow(1 + pi * gammaRPi - tau * pi * gammaRPiTau, 2) /
                                      (Math.Pow(tau, 2) * (gamma0TauTau + gammaRTauTau));

            double denominator = denominatorTerm1 + denominatorTerm2;

            return Math.Sqrt(rgas_water * temperature * numerator / denominator);
        }
        private double SpsoundReg3(double temperature, double density)
        {
            // Speed of sound in region 3
            // Returns spsoundreg3 in m/s
            // temperature in K
            // density in kg/m³

            double tau = tc_water / temperature;
            double delta = density / dc_water;

            double fidelta = Fideltareg3(tau, delta);
            double fideltadelta = Fideltadeltareg3(tau, delta);
            double fideltatau = Fideltataureg3(tau, delta);
            double fitautau = Fitautaureg3(tau, delta);

            double numerator = delta * fidelta - delta * tau * fideltatau;
            double denominatorTerm1 = 2 * delta * fidelta + Math.Pow(delta, 2) * fideltadelta;
            double denominatorTerm2 = Math.Pow(numerator, 2) / (Math.Pow(tau, 2) * fitautau);

            return Math.Sqrt(rgas_water * temperature * (denominatorTerm1 - denominatorTerm2));
        }
        public double SpsoundW(double temperature, double pressure)
        {
              // Speed of sound in water or steam
              // Returns spsoundW in m/s
              // temperature in K
              // pressure in bar
              // If out of range: returns -1
         
              if (temperature >= 273.15 && temperature <= 623.15 && pressure >= PSatW(temperature) && pressure <= 1000.0)
              {
                  // Region 1 - Liquid compressed
                  return SpsoundReg1(temperature, pressure);
              }
              else if ((temperature >= 273.15 && temperature <= 623.15 && pressure > 0 && pressure <= PSatW(temperature)) ||
                       (temperature > 623.15 && temperature <= 863.15 && pressure > 0 && pressure <= pBound(temperature)) ||
                       (temperature > 863.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0))
              {
                  // Region 2 - Vapor superheated
                  return SpsoundReg2(temperature, pressure);
              }
              else if (temperature >= 623.15 && temperature <= tBound(pressure) &&
                       pressure >= pBound(temperature) && pressure <= 1000.0)
              {
                  // Region 3 - Critical region
                  double density = DensReg3(temperature, pressure);
                  return SpsoundReg3(temperature, density);
              }
              else
              {
                  // Outside valid range
                  return -1;
              }
          }
        public double ViscW(double temperature, double pressure)
        {
            // Dynamic viscosity of water or steam
            // Returns viscW in Pa·s
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0)
            {
                double density = DensW(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else
            {
                return -1; // Out of range
            }
        }
        public double ThconW(double temperature, double pressure)
        {
            // Thermal conductivity of water or steam
            // Returns thconW in W/(m·K)
            // temperature in K
            // pressure in bar
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 1073.15 && pressure > 0 && pressure <= 1000.0)
            {
                double density = DensW(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else
            {
                return -1; // Out of range
            }
        }
        public double DensSatLiqTW(double temperature)
        {
            // Density of saturated liquid water as a function of temperature
            // Returns densSatLiqTW in kg/m³
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                return 1 / VolReg1(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                return DensReg3(temperature, pressure);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double DensSatVapTW(double temperature)
        {
            // Density of saturated steam as a function of temperature
            // Returns densSatVapTW in kg/m³
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Vapor superheated
                double pressure = PSatW(temperature);
                return 1 / VolReg2(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                return DensReg3(temperature, pressure);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double DensSatLiqPW(double pressure)
        {
            // Density of saturated liquid water as a function of pressure
            // Returns densSatLiqPW in kg/m³
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                return 1 / VolReg1(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                return DensReg3(temperature, pressure);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double DensSatVapPW(double pressure)
        {
            // Density of saturated steam as a function of pressure
            // Returns densSatVapPW in kg/m³
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                return 1 / VolReg2(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                return DensReg3(temperature, pressure);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnergySatLiqTW(double temperature)
        {
            // Specific internal energy of saturated liquid water as a function of temperature
            // Returns energySatLiqTW in kJ/kg
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                return EnergyReg1(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                return EnergyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnergySatVapTW(double temperature)
        {
            // Specific internal energy of saturated steam as a function of temperature
            // Returns energySatVapTW in kJ/kg
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Vapor superheated
                double pressure = PSatW(temperature);
                return EnergyReg2(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                return EnergyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnergySatLiqPW(double pressure)
        {
            // Specific internal energy of saturated liquid water as a function of pressure
            // Returns energySatLiqPW in kJ/kg
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                return EnergyReg1(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                return EnergyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnergySatVapPW(double pressure)
        {
            // Specific internal energy of saturated steam as a function of pressure
            // Returns energySatVapPW in kJ/kg
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                return EnergyReg2(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                return EnergyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EntropySatLiqTW(double temperature)
        {
            // Specific entropy of saturated liquid water as a function of temperature
            // Returns entropySatLiqTW in kJ/(kg·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                return EntropyReg1(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                return EntropyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EntropySatVapTW(double temperature)
        {
            // Specific entropy of saturated steam as a function of temperature
            // Returns entropySatVapTW in kJ/(kg·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Superheated vapor
                double pressure = PSatW(temperature);
                return EntropyReg2(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                return EntropyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EntropySatLiqPW(double pressure)
        {
            // Specific entropy of saturated liquid water as a function of pressure
            // Returns entropySatLiqPW in kJ/(kg·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                return EntropyReg1(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                return EntropyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EntropySatVapPW(double pressure)
        {
            // Specific entropy of saturated steam as a function of pressure
            // Returns entropySatVapPW in kJ/(kg·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                return EntropyReg2(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                return EntropyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnthalpySatLiqTW(double temperature)
        {
            // Specific enthalpy of saturated liquid water as a function of temperature
            // Returns enthalpySatLiqTW in kJ/kg
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                return EnthalpyReg1(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                return EnthalpyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnthalpySatVapTW(double temperature)
        {
            // Specific enthalpy of saturated steam as a function of temperature
            // Returns enthalpySatVapTW in kJ/kg
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Superheated vapor
                double pressure = PSatW(temperature);
                return EnthalpyReg2(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                return EnthalpyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnthalpySatLiqPW(double pressure)
        {
            // Specific enthalpy of saturated liquid water as a function of pressure
            // Returns enthalpySatLiqPW in kJ/kg
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                return EnthalpyReg1(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                return EnthalpyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double EnthalpySatVapPW(double pressure)
        {
            // Specific enthalpy of saturated steam as a function of pressure
            // Returns enthalpySatVapPW in kJ/kg
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                return EnthalpyReg2(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                return EnthalpyReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CpSatLiqTW(double temperature)
        {
            // Specific isobaric heat capacity of saturated liquid water as a function of temperature
            // Returns cpSatLiqTW in kJ/(kg·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                return CpReg1(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                return CpReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CpSatVapTW(double temperature)
        {
            // Specific isobaric heat capacity of saturated steam as a function of temperature
            // Returns cpSatVapTW in kJ/(kg·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Superheated vapor
                double pressure = PSatW(temperature);
                return CpReg2(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                return CpReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CpSatLiqPW(double pressure)
        {
            // Specific isobaric heat capacity of saturated liquid water as a function of pressure
            // Returns cpSatLiqPW in kJ/(kg·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                return CpReg1(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                return CpReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CpSatVapPW(double pressure)
        {
            // Specific isobaric heat capacity of saturated steam as a function of pressure
            // Returns cpSatVapPW in kJ/(kg·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                return CpReg2(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                return CpReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CvSatLiqTW(double temperature)
        {
            // Specific isochoric heat capacity of saturated liquid water as a function of temperature
            // Returns cvSatLiqTW in kJ/(kg·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                return CvReg1(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                return CvReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CvSatVapTW(double temperature)
        {
            // Specific isochoric heat capacity of saturated steam as a function of temperature
            // Returns cvSatVapTW in kJ/(kg·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Superheated vapor
                double pressure = PSatW(temperature);
                return CvReg2(temperature, pressure);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                return CvReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CvSatLiqPW(double pressure)
        {
            // Specific isochoric heat capacity of saturated liquid water as a function of pressure
            // Returns cvSatLiqPW in kJ/(kg·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                return CvReg1(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                return CvReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double CvSatVapPW(double pressure)
        {
            // Specific isochoric heat capacity of saturated steam as a function of pressure
            // Returns cvSatVapPW in kJ/(kg·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                return CvReg2(temperature, pressure);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                return CvReg3(temperature, density);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ViscSatLiqTW(double temperature)
        {
            // Dynamic viscosity of saturated liquid water as a function of temperature
            // Returns viscSatLiqTW in Pa·s
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                double density = 1 / VolReg1(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ViscSatVapTW(double temperature)
        {
            // Dynamic viscosity of saturated steam as a function of temperature
            // Returns viscSatVapTW in Pa·s
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Superheated vapor
                double pressure = PSatW(temperature);
                double density = 1 / VolReg2(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ViscSatLiqPW(double pressure)
        {
            // Dynamic viscosity of saturated liquid water as a function of pressure
            // Returns viscSatLiqPW in Pa·s
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                double density = 1 / VolReg1(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ViscSatVapPW(double pressure)
        {
            // Dynamic viscosity of saturated steam as a function of pressure
            // Returns viscSatVapPW in Pa·s
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                double density = 1 / VolReg2(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.000055071 * PsiVisc(tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ThconSatLiqTW(double temperature)
        {
            // Thermal conductivity of saturated liquid water as a function of temperature
            // Returns thconSatLiqTW in W/(m·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 1 - Liquid compressed
                double pressure = PSatW(temperature);
                double density = 1 / VolReg1(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature);
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ThconSatVapTW(double temperature)
        {
            // Thermal conductivity of saturated steam as a function of temperature
            // Returns thconSatVapTW in W/(m·K)
            // temperature in K
            // If out of range: returns -1

            if (temperature >= 273.15 && temperature <= 623.15)
            {
                // Region 2 - Superheated vapor
                double pressure = PSatW(temperature);
                double density = 1 / VolReg2(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                pressure -= 0.0001 * pressure;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else if (temperature > 623.15 && temperature <= tc_water)
            {
                // Region 3 - Critical region
                double pressure = PSatW(temperature) - 0.00001;
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ThconSatLiqPW(double pressure)
        {
            // Thermal conductivity of saturated liquid water as a function of pressure
            // Returns thconSatLiqPW in W/(m·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 1 - Liquid compressed
                double temperature = TSatW(pressure);
                double density = 1 / VolReg1(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure += 0.00001;
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
        public double ThconSatVapPW(double pressure)
        {
            // Thermal conductivity of saturated steam as a function of pressure
            // Returns thconSatVapPW in W/(m·K)
            // pressure in bar
            // If out of range: returns -1

            double psat_273 = PSatW(273.15);
            double psat_623 = PSatW(623.15);

            if (pressure >= psat_273 && pressure <= psat_623)
            {
                // Region 2 - Superheated vapor
                double temperature = TSatW(pressure);
                double density = 1 / VolReg2(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                pressure -= 0.0001 * pressure;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else if (pressure > psat_623 && pressure <= pc_water)
            {
                // Region 3 - Critical region
                double temperature = TSatW(pressure);
                pressure -= 0.00001;
                double density = DensReg3(temperature, pressure);
                double delta = density / 317.763;
                double tau = 647.226 / temperature;

                return 0.4945 * LambThCon(temperature, pressure, tau, delta);
            }
            else
            {
                return -1; // Out of valid range
            }
        }
    }
}
