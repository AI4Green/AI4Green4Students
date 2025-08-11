import { createContext, useContext } from "react";

export const SectionFormContext = createContext({});

export const useSectionForm = () => useContext(SectionFormContext);
