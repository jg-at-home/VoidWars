namespace VoidWars {
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

                case AuxType.ChaffLauncher:
                    return new ChaffLauncher(auxClass);

                case AuxType.MineLauncher:
                    return new MineLauncher(auxClass);

                case AuxType.EMPGenerator:
                    return new EMPGenerator(auxClass);

                default:
                    return new AuxItem(auxClass);
            }
        }
    }
}