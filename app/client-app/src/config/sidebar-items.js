import {
  REGISTRATION_RULES_PERMISSIONS,
  USERMANAGEMENT_PERMISSIONS,
} from "constants";
import { FaPencilRuler, FaUserCog } from "react-icons/fa";

export const getSidebarItems = (t) => [
  {
    label: t("adminMenu.menuList.userManagement"),
    path: "/admin/usermanagement",
    icon: FaUserCog,
    permission: USERMANAGEMENT_PERMISSIONS,
  },
  {
    label: t("adminMenu.menuList.registrationRule"),
    path: "/admin/registrationrule",
    icon: FaPencilRuler,
    permission: REGISTRATION_RULES_PERMISSIONS,
  },
];
