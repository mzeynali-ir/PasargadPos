using PcPosClassLibrary;

namespace IMustafa_PasargadPos
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Pasargad Pos!");

            int port = 7000;
            string portID = "192.168.1.102";
            PasargadPos pos = new PasargadPos(port, portID);

            Console.WriteLine("waiting for pay ...");

            long amount = 10002;

            var res = await pos.SaleAsync(amount);

            if (res.Success)
                Console.WriteLine("successed");
            else
                Console.WriteLine("Failed");

            Console.ReadLine();
        }
    }

    public class PasargadPosResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class PasargadPos
    {
        private readonly PCPOS pos;
        public PasargadPos(int port, string portID)
        {
            pos = new PCPOS(port, portID);
        }

        public Task<PasargadPosResult> SaleAsync(long amoutePerRials)
        {
            int timeOutInPerSecound = 500;
            pos.SetLanReceiveTimeout(timeOutInPerSecound);
            RecievedData? res = null;
            try
            {
                res = pos.SyncSale(amoutePerRials);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new PasargadPosResult() { Success = false, Message = "مشکلی پیش آمده است" });
            }
            finally
            {
                pos.Close();
            }

            // تراکنش ناموفق است و ارور دارد
            if (res.HasError)
                return Task.FromResult(new PasargadPosResult() { Success = false, Message = res.ErrorMessage });

            // مبلغ تراکنش متفاوت است
            if (res.Amount != amoutePerRials.ToString())
                return Task.FromResult(new PasargadPosResult() { Success = false, Message = "مشکلی پیش آمده است" });

            return Task.FromResult(new PasargadPosResult() { Success = true, Message = res.ErrorMessage });
        }
    }
}
