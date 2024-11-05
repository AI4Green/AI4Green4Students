import {
  USERMANAGEMENT_PERMISSIONS,
  REGISTRATION_RULES_PERMISSIONS,
} from "constants";
import { FaUserCog, FaPencilRuler } from "react-icons/fa";

export const getSidebarItems = (t) => [
  {
    name: t("adminMenu.menuList.userManagement"),
    path: "/admin/usermanagement",
    icon: FaUserCog,
    permission: USERMANAGEMENT_PERMISSIONS,
  },
  {
    name: t("adminMenu.menuList.registrationRule"),
    path: "/admin/registrationrule",
    icon: FaPencilRuler,
    permission: REGISTRATION_RULES_PERMISSIONS,
  },
];
