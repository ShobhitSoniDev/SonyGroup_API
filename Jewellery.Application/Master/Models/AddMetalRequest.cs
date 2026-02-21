namespace Jewellery.Application.Master.Models
{
    public class AddMetalRequest
    {
        public string MetalName { get; set; }
        public decimal Purity { get; set; }
        public int CreatedBy { get; set; }
    }
}
