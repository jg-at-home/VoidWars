﻿namespace VoidWars {
    /// <summary>
    /// Factory class for creating auxiliary items.
    /// </summary>
    public static class AuxItems {
        public static AuxItem CreateAux(AuxiliaryClass auxClass) {
            switch(auxClass.ItemType) {
                case AuxType.FlareLauncher:
                    return new FlareLauncher(auxClass);

                case AuxType.ERBInducer:
                    return new Teleporter(auxClass);

                default:
                    return new AuxItem(auxClass);
            }
        }
    }
}