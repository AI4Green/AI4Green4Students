import { createContext, useContext } from "react";

export const MutateSectionFormContext = createContext({});

export const useMutateSectionForm = () => useContext(MutateSectionFormContext);
