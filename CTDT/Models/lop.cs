//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CTDT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class lop
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public lop()
        {
            this.sinhvien = new HashSet<sinhvien>();
        }
    
        public int id_lop { get; set; }
        public int id_ctdt { get; set; }
        public string ma_lop { get; set; }
        public int ngaycapnhat { get; set; }
        public int ngaytao { get; set; }
    
        public virtual ctdt ctdt { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sinhvien> sinhvien { get; set; }
    }
}