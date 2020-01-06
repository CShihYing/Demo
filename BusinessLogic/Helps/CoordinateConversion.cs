using System;

namespace BusinessLogic.Helps
{
    /// <summary>
    /// 坐標系轉換
    /// </summary>
    public class CoordinateConversion
    {
        static double _cos675 = 0.3826834323650897717284599840304;
        static double _pi = 3.14159265358979323;
        static double _halfPi = 1.570796326794896615;
        static double _degRad = 0.01745329251994329572;
        static double _radDeg = 57.295779513082321031;
        static double _adC = 1.0026000;

        static double _twd67A = 6378160.0;
        static double _twd67B = 6356774.7192;
        static double _twd67Ecc = 0.00669454185458;
        static double _twd67Ecc2 = 0.00673966079586;
        static double _twd67Dx = -752.32; // different from garmin and already knowned value, but those all value only
        static double _twd67Dy = -361.32; // got 5-15m accuracy. the real offical value is holded by somebody and not

        static double _twd67Dz = -180.51; // release to public. if can got more enough twd67/twd97 control point coordinare,

        static double _twd67Rx = -0.00000117; // then we can calculate a better value than now.
        static double _twd67Ry = 0.00000184; // 
        static double _twd67Rz = 0.00000098; // and, also lack twd67/twd97 altitude convertion value...
        static double _twd67S = 0.00002329; //


        static double _twd97A = 6378137.0;
        static double _twd97B = 6356752.3141;
        static double _twd97Ecc = 0.00669438002290;
        static double _twd97Ecc2 = 0.00673949677556;

        static double _twd67Tm2 = 0.9999; // TWD67->TM2 scale
        static double _twd97Tm2 = 0.9999; // TWD97->TM2 scale

        // center longitude of taiwan is 121, for penghu is 119
        static double _dx = 250000; // TM2 in Taiwan should add 250000

        static double Mercator(double y, double a, double ecc)
        {
            if (y == 0.0)
            {
                return 0.0;
            }
            else
            {
                return a * (
                           (1.0 - ecc / 4.0 - 3.0 * ecc * ecc / 64.0 - 5.0 * ecc * ecc * ecc / 256.0) * y -
                           (3.0 * ecc / 8.0 + 3.0 * ecc * ecc / 32.0 + 45.0 * ecc * ecc * ecc / 1024.0) * Math.Sin(2.0 * y) +
                           (15.0 * ecc * ecc / 256.0 + 45.0 * ecc * ecc * ecc / 1024.0) * Math.Sin(4.0 * y) -
                           (35.0 * ecc * ecc * ecc / 3072.0) * Math.Sin(6.0 * y));
            }
        }

        static bool ToTm2(double a, double ecc, double ecc2, double lat, double lon, double scale, double inx, double iny, ref double ox, ref double oy) //經緯度轉TM2度
        {
            bool result = true;

            double x0, y0, x1, y1, m0, m1;
            double n, t, c, A;
            double xx = 0; double yy = 0;



            try
            {
                x0 = inx * _degRad;
                y0 = iny * _degRad;

                x1 = lon * _degRad;
                y1 = lat * _degRad;

                m0 = Mercator(y1, a, ecc);
                m1 = Mercator(y0, a, ecc);

                n = a / Math.Sqrt(1 - ecc * Math.Pow(Math.Sin(y0), 2.0));
                t = Math.Pow(Math.Tan(y0), 2.0);
                c = ecc2 * Math.Pow(Math.Cos(y0), 2.0);
                A = (x0 - x1) * Math.Cos(y0);

                xx = scale * n * (A + (1.0 - t + c) * A * A * A / 6.0 + (5.0 - 18.0 * t + t * t + 72.0 * c - 58.0 * ecc2) * Math.Pow(A, 5.0) / 120.0) + 250000;
                yy = scale * (m1 - m0 + n * Math.Tan(y0) * (A * A / 2.0 + (5.0 - t + 9.0 * c + 4 * c * c) * Math.Pow(A, 4.0) / 24.0 + (61.0 - 58.0 * t + t * t + 600.0 * c - 330.0 * ecc2) * Math.Pow(A, 6.0) / 720.0));



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = false;
            }


            ox = xx;
            oy = yy;

            return result;
        }

        static int FromTm2(double a, double ecc, double ecc2, double lat, double lon, double scale, double x, double y, ref double ox, ref double oy) //TM2 to 經緯度
        {
            double x0, y0, x1, y1, phi, m, m0, mu, e1;
            double c1, t1, n1, r1, d;
            double xx, yy;

            x0 = x - _dx;
            y0 = y;

            x1 = lon * _degRad;
            y1 = lat * _degRad;

            m0 = Mercator(y1, a, ecc);
            m = m0 + y0 / scale;

            e1 = (1.0 - Math.Sqrt(1.0 - ecc)) / (1.0 + Math.Sqrt(1.0 - ecc));
            mu = m / (a * (1.0 - ecc / 4.0 - 3.0 * ecc * ecc / 64.0 - 5.0 * ecc * ecc * ecc / 256.0));

            phi = mu + (3.0 * e1 / 2.0 - 27.0 * Math.Pow(e1, 3.0) / 32.0) * Math.Sin(2.0 * mu)
                     + (21.0 * e1 * e1 / 16.0 - 55.0 * Math.Pow(e1, 4.0) / 32.0) * Math.Sin(4.0 * mu)
                     + 151.0 * Math.Pow(e1, 3.0) / 96.0 * Math.Sin(6.0 * mu) + 1097.0 * Math.Pow(e1, 4.0) / 512.0 * Math.Sin(8.0 * mu);

            c1 = ecc2 * Math.Pow(Math.Cos(phi), 2.0);
            t1 = Math.Pow(Math.Tan(phi), 2.0);
            n1 = a / Math.Sqrt(1 - ecc * Math.Pow(Math.Sin(phi), 2.0));
            r1 = a * (1.0 - ecc) / Math.Pow(1.0 - ecc * Math.Pow(Math.Sin(phi), 2.0), 1.5);
            d = x0 / (n1 * scale);

            xx = (x1 + (d - (1.0 + 2.0 * t1 + c1) * Math.Pow(d, 3.0) / 6.0
                        + (5.0 - 2.0 * c1 + 28.0 * t1 - 3.0 * c1 * c1 + 8.0 * ecc2 + 24.0 * t1 * t1) * Math.Pow(d, 5.0) /
                        120.0) / Math.Cos(phi)) * _radDeg;
            yy = (phi - n1 * Math.Tan(phi) / r1 * (d * d / 2.0 - (5.0 + 3.0 * t1 + 10.0 * c1 - 4.0 * c1 * c1 - 9.0 * ecc2)
                                              * Math.Pow(d, 4.0) / 24.0 +
                                              (61.0 + 90.0 * t1 + 298.0 * c1 + 45.0 * t1 * t1 - 252.0 * ecc2 -
                                               3.0 * c1 * c1) * Math.Pow(d, 6.0) / 72.0)) * _radDeg;

            ox = xx;
            oy = yy;

            return 1;
            //	return [xx,yy];
        }

        /// <summary>
        /// WGS84轉TWD97坐標系
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        /// <returns></returns>
        public static bool Lat2Tm97(double x, double y, ref double ox, ref double oy)
        {
            return ToTm2(_twd97A, _twd97Ecc, _twd97Ecc2, 0, 121, _twd97Tm2, x, y, ref ox, ref oy);
        }

        /// <summary>
        /// TM97 to wgs84
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        /// <returns></returns>
        public static int Tm972Lat(double x, double y, ref double ox, ref double oy)
        {
            return FromTm2(_twd97A, _twd97Ecc, _twd97Ecc2, 0, 121, _twd97Tm2, x, y, ref ox, ref oy);
        }

        /// <summary>
        /// wgs84 to 97
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        /// <returns></returns>
        public static bool lat2tm97_119(double x, double y, ref double ox, ref double oy)
        {
            return ToTm2(_twd97A, _twd97Ecc, _twd97Ecc2, 0, 119, _twd97Tm2, x, y, ref ox, ref oy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        /// <returns></returns>
        public static int tm972lat_119(double x, double y, ref double ox, ref double oy)
        {
            return FromTm2(_twd97A, _twd97Ecc, _twd97Ecc2, 0, 119, _twd97Tm2, x, y, ref ox, ref oy);
        }

        public static int Tm67297(double x, double y, ref double ox, ref double oy)
        {
            double a = 0.00001549;
            double b = 0.000006521;

            //	X67 = X97 - 807.8 - A * X97 - B * Y97
            //	Y67 = Y97 + 248.6 - A * Y97 - B * X97
            double x67 = x;
            double y67 = y;
            double x97 = x67 + 807.8 + a * x67 + b * y67;
            double y97 = y67 - 248.6 + a * y67 + b * x67;

            ox = x97;
            oy = y97;
            return 1;
        }

        public static int Tm97To67(double x, double y, ref double ox, ref double oy)
        {
            double a = 0.00001549;
            double b = 0.000006521;

            //	X67 = X97 - 807.8 - A * X97 - B * Y97
            //	Y67 = Y97 + 248.6 - A * Y97 - B * X97

            ox = x - 807.8 - a * x - b * y;
            oy = y + 248.6 - a * y - b * x;

            return 1;
        }

        /// <summary>
        /// wgs84 to 67
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        /// <returns></returns>
        public static bool Lat2Tm67(double x, double y, ref double ox, ref double oy)
        {
            double x1 = 0;
            double y1 = 0;
            Lat2Tm97(x, y, ref x1, ref y1);
            Tm97To67(x1, y1, ref ox, ref oy);
            return true;
        }

    }
}
