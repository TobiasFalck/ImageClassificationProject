using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    class InputData
    {
        [VectorType(1)]
        public VBuffer<Single> Image; // the VBuffer<> type actually represents the data
    }
}
