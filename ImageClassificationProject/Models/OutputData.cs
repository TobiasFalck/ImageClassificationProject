using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    class OutputData
    {
        // THE MAGICAL FIX: attribute specifies vector type of unknown length (i.e. VarVector)
        [VectorType()]
        public VBuffer<Byte> Image; // the VBuffer<> type actually represents the data
    }
}
