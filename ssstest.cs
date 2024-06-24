namespace new2
{
    public class EoHFCNetwork
    {
        public CMTSVendor CMTSVendor { get; set; }
        public RouterVendor Router1Vendor { get; set; }
        public RouterVendor? Router2Vendor { get; set; }
        public RouterType Router1Type { get; set; }
        public RouterType? Router2Type { get; set; }
        public ICMTSParser CMTSParser { get; set; }
        public IRouterParser Router1Parser { get; set; }
        public IRouterParser Router2Parser { get; set; }

        public EoHFCNetwork(CMTSVendor cmtsVendor, RouterVendor router1Vendor, RouterType router1Type, RouterVendor? router2Vendor,
                       RouterType? router2Type, ICMTSParser cmtsParser, IRouterParser router1Parser, IRouterParser router2Parser)
        {
            CMTSVendor = cmtsVendor;
            Router1Vendor = router1Vendor;
            Router2Vendor = router2Vendor;
            Router1Type = router1Type;
            Router2Type = router2Type;
            CMTSParser = cmtsParser;
            Router1Parser = router1Parser;
            Router2Parser = router2Parser;
        }
    }


    public enum RouterType
    {
        SUR = 1,
        SOAG = 2,
        SSAG = 3,
        CEG = 4
    }

    public enum CMTSVendor
    {
        Cisco = 1,
        Arrista = 2,
        VCMTS = 3
    }

    public enum RouterVendor
    {
        Cisco = 1,
        Juniper = 2,
        Nokia = 3
    }

    public enum ServiceType
    {
        EAEPL = 1,
        ENS = 2,
        EDIA = 3
    }

    public class SingleMove
    {
        public SingleMove() { }

        public void PerformOperation()
        {
            string sourceCMTS = "Cisco"; // Example input
            string sourceRouter1 = "SUR"; // Example input
            string? sourceRouter2 = null; // Example input
            string destinationCMTS = "Arrista"; // Example input
            string destinationRouter1 = "SOAG"; // Example input
            string destinationRouter2 = "CEG"; // Example input

            // Prevalidation (e.g., ensure inputs are not null, check for valid values, etc.)

            // Identify vendors for the devices
            CMTSVendor sourceCMTSVendor = DetectCMTSVendor(sourceCMTS);
            RouterVendor sourceRouter1Vendor = DetectRouterVendor("Cisco"); // Example input
            RouterType sourceRouter1Type = DetectRouterType(sourceRouter1);
            RouterVendor? sourceRouter2Vendor = sourceRouter2 != null ? DetectRouterVendor("Nokia") : (RouterVendor?)null; // Example input
            RouterType? sourceRouter2Type = sourceRouter2 != null ? DetectRouterType(sourceRouter2) : (RouterType?)null;

            CMTSVendor destinationCMTSVendor = DetectCMTSVendor(destinationCMTS);
            RouterVendor destinationRouter1Vendor = DetectRouterVendor("Juniper"); // Example input
            RouterType destinationRouter1Type = DetectRouterType(destinationRouter1);
            RouterVendor? destinationRouter2Vendor = destinationRouter2 != null ? DetectRouterVendor("Nokia") : (RouterVendor?)null; // Example input
            RouterType? destinationRouter2Type = destinationRouter2 != null ? DetectRouterType(destinationRouter2) : (RouterType?)null;

            // Detect parsers based on vendors
            ICMTSParser sourceCMTSParser = GetCMTSParser(sourceCMTSVendor);
            IRouterParser sourceRouter1Parser = GetRouterParser(sourceRouter1Type);
            IRouterParser? sourceRouter2Parser = sourceRouter2Type.HasValue ? GetRouterParser(sourceRouter2Type.Value) : null;
            ICMTSParser destinationCMTSParser = GetCMTSParser(destinationCMTSVendor);
            IRouterParser destinationRouter1Parser = GetRouterParser(destinationRouter1Type);
            IRouterParser? destinationRouter2Parser = destinationRouter2Type.HasValue ? GetRouterParser(destinationRouter2Type.Value) : null;

            // Create source and destination network configurations
            EoHFCNetwork sourceNetwork = new EoHFCNetwork(
                sourceCMTSVendor, sourceRouter1Vendor, sourceRouter1Type, sourceRouter2Vendor, sourceRouter2Type, sourceCMTSParser, sourceRouter1Parser, sourceRouter2Parser
            );

            EoHFCNetwork destinationNetwork = new EoHFCNetwork(
                destinationCMTSVendor, destinationRouter1Vendor, destinationRouter1Type, destinationRouter2Vendor, destinationRouter2Type, destinationCMTSParser, destinationRouter1Parser, destinationRouter2Parser
            );

            // Determine service type from source configuration
            ServiceType serviceType = GetServiceType(sourceRouter1);

            // Factory to get service collector based on service type
            IServiceMigrator ServiceMigrator = GetServiceMigrator(serviceType, sourceNetwork, destinationNetwork);

            // Perform the service collection operation (not implemented in this example)
            // ServiceMigrator.Collect();

            // Output for verification (for demonstration purposes)
            Console.WriteLine($"Source CMTS: {sourceCMTS}, Source Router1: {sourceRouter1}, Source Router2: {sourceRouter2}");
            Console.WriteLine($"Destination CMTS: {destinationCMTS}, Destination Router1: {destinationRouter1}, Destination Router2: {destinationRouter2}");
            Console.WriteLine($"Service Collector: {ServiceMigrator.GetType().Name}");
        }

        private CMTSVendor DetectCMTSVendor(string cmts)
        {
            return cmts switch
            {
                "Cisco" => CMTSVendor.Cisco,
                "Arrista" => CMTSVendor.Arrista,
                "VCMTS" => CMTSVendor.VCMTS,
                _ => throw new ArgumentException("Invalid CMTS vendor")
            };
        }

        private RouterVendor DetectRouterVendor(string router)
        {
            return router switch
            {
                "Cisco" => RouterVendor.Cisco,
                "Juniper" => RouterVendor.Juniper,
                "Nokia" => RouterVendor.Nokia,
                _ => throw new ArgumentException("Invalid Router vendor")
            };
        }

        private RouterType DetectRouterType(string router)
        {
            return router switch
            {
                "SUR" => RouterType.SUR,
                "SOAG" => RouterType.SOAG,
                "SSAG" => RouterType.SSAG,
                "CEG" => RouterType.CEG,
                _ => throw new ArgumentException("Invalid Router type")
            };
        }

        private ICMTSParser GetCMTSParser(CMTSVendor vendor)
        {
            return vendor switch
            {
                CMTSVendor.Cisco => new CiscoCMTSParser(),
                CMTSVendor.Arrista => new ArristaParser(),
                CMTSVendor.VCMTS => new VCMTSParser(),
                _ => throw new ArgumentException("Invalid CMTS vendor")
            };
        }

        private IRouterParser GetRouterParser(RouterType routerType)
        {
            return routerType switch
            {
                RouterType.SUR => new ICiscoRouterParser(),
                RouterType.SOAG => new IJuniperRouterParser(),
                RouterType.SSAG => new ICiscoRouterParser(),
                RouterType.CEG => new NokiaRouterParser(),
                _ => throw new ArgumentException("Invalid Router type")
            };
        }

        private ServiceType GetServiceType(string router)
        {
            // Example logic for determining service type based on router types
           
            else
            {
                throw new ArgumentException("Invalid router combination for service type");
            }
        }

        private IServiceMigrator GetServiceMigrator(ServiceType serviceType, EoHFCNetwork sourceNetwork, EoHFCNetwork destinationNetwork)
        {
            return serviceType switch
            {
                ServiceType.EAEPL => new EAEPLServiceMigrator(sourceNetwork, destinationNetwork),
                ServiceType.ENS => new ENSServiceMigrator(sourceNetwork, destinationNetwork),
                ServiceType.EDIA => new EDIAServiceMigrator(sourceNetwork, destinationNetwork),
                _ => throw new ArgumentException("Invalid service type")
            };
        }
    }

    // Define the parsers and service collectors

    public interface ICMTSParser { }
    public class CiscoCMTSParser : ICMTSParser { }
    public class ArristaParser : ICMTSParser { }
    public class VCMTSParser : ICMTSParser { }

    public interface IRouterParser { }
    public class NokiaRouterParser : IRouterParser { }
    public class ICiscoRouterParser : IRouterParser { }
    public class IJuniperRouterParser : IRouterParser { }

    public interface IServiceMigrator
    {
        void SourceCollector();
        void DestinationCollector();
        // for New configuration we can use scriban template engine instead manually parsing the information 
        // https://scribanonline.azurewebsites.net/
        void NewConfiguration();
        void CleanUpConfiguration();
        // Existing Logic can be reused
        void ApplyConfiguration();
        void MoveValidation();
        void SendEmail();


    }

    public class EAEPLServiceMigrator : IServiceMigrator
    {
        private EoHFCNetwork _sourceNetwork;
        private EoHFCNetwork _destinationNetwork;

        public EAEPLServiceMigrator(EoHFCNetwork sourceNetwork, EoHFCNetwork destinationNetwork)
        {
            _sourceNetwork = sourceNetwork;
            _destinationNetwork = destinationNetwork;
        }

        public void ApplyConfiguration()
        {
            throw new NotImplementedException();
        }

        public void CleanUpConfiguration()
        {
            throw new NotImplementedException();
        }

        public void DestinationCollector()
        {
            throw new NotImplementedException();
        }

        public void MoveValidation()
        {
            throw new NotImplementedException();
        }

        public void NewConfiguration()
        {
            throw new NotImplementedException();
        }

        public void SendEmail()
        {
            throw new NotImplementedException();
        }

        public void SourceCollector()
        {
            // Implementation of the collection process
        }
    }

    public class ENSServiceMigrator : IServiceMigrator
    {
        private EoHFCNetwork _sourceNetwork;
        private EoHFCNetwork _destinationNetwork;

        public ENSServiceMigrator(EoHFCNetwork sourceNetwork, EoHFCNetwork destinationNetwork)
        {
            _sourceNetwork = sourceNetwork;
            _destinationNetwork = destinationNetwork;
        }

        public void ApplyConfiguration()
        {
            throw new NotImplementedException();
        }

        public void CleanUpConfiguration()
        {
            throw new NotImplementedException();
        }

        public void DestinationCollector()
        {
            throw new NotImplementedException();
        }

        public void MoveValidation()
        {
            throw new NotImplementedException();
        }

        public void NewConfiguration()
        {
            throw new NotImplementedException();
        }

        public void SendEmail()
        {
            throw new NotImplementedException();
        }

        public void SourceCollector()
        {
            // Implementation of the collection process
        }
    }

    public class EDIAServiceMigrator : IServiceMigrator
    {
        private EoHFCNetwork _sourceNetwork;
        private EoHFCNetwork _destinationNetwork;

        public EDIAServiceMigrator(EoHFCNetwork sourceNetwork, EoHFCNetwork destinationNetwork)
        {
            _sourceNetwork = sourceNetwork;
            _destinationNetwork = destinationNetwork;
        }

        public void SourceCollector()
        {
            // Implementation of the collection process
        }

        public void DestinationCollector()
        {
            throw new NotImplementedException();
        }

        public void ApplyConfiguration()
        {
            throw new NotImplementedException();
        }

        public void MoveValidation()
        {
            throw new NotImplementedException();
        }

        public void SendEmail()
        {
            throw new NotImplementedException();
        }

        public void NewConfiguration()
        {
            throw new NotImplementedException();
        }

        public void CleanUpConfiguration()
        {
            throw new NotImplementedException();
        }
    }
}
