using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Diamond.Common.Objects;

namespace IFM.DataServicesCore.CommonObjects.OMP
{
    public class PackagePart
    {
        public int packagepart_num { get; set; }
        public string PackagePartName { get; set; }

        public int VersionId { get; set; }

        public static List<PackagePart> GetPackagePart(int PolicyId, int PolicyImageNum, InsCollection<global::Diamond.Common.Objects.Policy.PackagePart> packageParts)
        {
            List<PackagePart> PackageParts = new List<PackagePart>();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(BusinessLogic.AppConfig.ConnDiamond))
            {
                conn.Open();
                using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "[dbo].[assp_PackagePartList_Load]";
                    cmd.Parameters.AddWithValue("@policy_id", PolicyId);
                    cmd.Parameters.AddWithValue("@policyimage_num", PolicyImageNum);
                    cmd.Parameters.AddWithValue("@calledfromclaims", 1);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (System.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                int versionId = 0;
                                var packagePartsList = packageParts.Where(pp => pp.PackagePartNum == reader.GetInt32(0));
                                if (packagePartsList.Any())
                                {
                                    versionId = packagePartsList.First().VersionId;
                                }
                                PackageParts.Add(new PackagePart() { packagepart_num = reader.GetInt32(0), PackagePartName = reader.GetString(1), VersionId = versionId });

                            }
                        }
                        foreach(var p in PackageParts)
                        {
                           
                            if (p.PackagePartName.Contains("Crime")) p.PackagePartName = "Crime";
                            if (p.PackagePartName.Contains("Garage")) p.PackagePartName = "Garage"; 
                            if (p.PackagePartName.Contains("General Liability")) p.PackagePartName = "General Liability";
                            if (p.PackagePartName.Contains("Inland Marine")) p.PackagePartName = "Inland Marine"; 
                            if (p.PackagePartName.Contains("Property")) p.PackagePartName = "Property";
                            
                        }
                    }
                 
                }

            }
            return PackageParts;
        }
    }
}
