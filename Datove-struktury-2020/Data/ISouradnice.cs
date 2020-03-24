using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    interface ISouradnice : IComparable
    {
        // jakakoliv trida implementuje-li rozhrani, stava se soucasne i datovym typem Interfacu
        //
        int vratX(); //get
        int vratY(); //get
        
    }
}
