//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DHDoiSim.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DMMucUuTien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DMMucUuTien()
        {
            this.Sim_Phieu = new HashSet<Sim_Phieu>();
        }
    
        public int ID { get; set; }
        public string StatusName { get; set; }
        public string GhiChu { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sim_Phieu> Sim_Phieu { get; set; }
    }
}
