import React from "react";
import { WasteIntensity } from "./WasteIntensity";
import { ReactionMassEfficiency } from "pages/GreenMetrics/ReactionMassEfficiency";
import { EFactor } from "./EFactor";

export default function GreenMetrics() {
  return (
    <>
      <WasteIntensity />
      <EFactor />
      <ReactionMassEfficiency />
    </>
  );
}
