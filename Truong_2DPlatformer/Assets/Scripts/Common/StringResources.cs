using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class StringResources
{
    public class ObjectIds
    {
        public const string ItemSlot1 = "c639f3bc-9971-4efc-baa3-a547b5bc1595";

        //triangle debug
        public const string Key_01 = "a5782b01-2fab-47ed-abf7-687f08721ad5";
        /// <summary>
        /// square debug
        /// </summary>
        public const string Key_02 = "060db457-f1fe-410b-b98b-abb884e8bb06";
        /// <summary>
        /// square to open cave
        /// </summary>
        public const string Key_03 = "fb714a85-0023-4026-8144-bd4fd2c6de9e";
        /// <summary>
        /// traingle inside cave
        /// </summary>
        public const string Key_04 = "1ac32f4d-f43e-43e3-b048-516ef01d3ec6";

        /// <summary>
        /// for key1
        /// </summary>
        public const string KeyPillar_01 = "160dfb9f-6218-4b29-a7ed-c50fde295b47";
        /// <summary>
        /// for key2
        /// </summary>
        public const string KeyPillar_02 = "88c0e01a-84c4-4a76-8488-aabda215bf3f";
        /// <summary>
        /// for key3
        /// </summary>
        public const string KeyPillar_03 = "c056ec14-993e-41be-bb62-4cdb9f392945";
        /// <summary>
        /// for key4
        /// </summary>
        public const string KeyPillar_04 = "c2b966ab-4319-433d-b819-5d7821d46d53";
    }

    public class HelpMessages
    {
        public const string Item_InvalidPlacement = "This is not a valid spot for this item";
    }

    public class System_Messages
    {
        /// <summary>
        /// {0} dropTarget.name
        /// {1} item.name
        /// {2} item.uid
        /// </summary>
        public const string DropTarget_ValidationFailedFormat =  "itemDropTarget:{0} is not compatible with this item:{1}[{2}]";
    }


}
