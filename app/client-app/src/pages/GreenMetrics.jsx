import { EFactor } from "components/GreenMetrics/EFactor";
import { ReactionMassEfficiency } from "components/GreenMetrics/ReactionMassEfficiency";
import { WasteIntensity } from "components/GreenMetrics/WasteIntensity";

export default function GreenMetrics() {
  return (
    <>
      <WasteIntensity />
      <EFactor />
      <ReactionMassEfficiency />
    </>
  );
}
