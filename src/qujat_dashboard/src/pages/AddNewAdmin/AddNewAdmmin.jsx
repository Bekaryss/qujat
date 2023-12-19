import React, { useState } from "react";
import { Box, Button } from "@mui/material";

import AddAdmin from "../../components/Modals/Admin/AddAdmin";
import TableAdmin from "../../components/Table/TableAdmin/TableAdmin";

const AddNewAdmmin = () => {
  const [isOpenAddAdmin, setIsOpenAddAdmin] = useState(false);

  const handleOpenAdd = () => {
    setIsOpenAddAdmin(true);
  };
  const handleCloseAdd = () => {
    setIsOpenAddAdmin(false);
  };

  return (
    <div className="home_container">
      <h1>Қолданушы</h1>
      <Box sx={{ display: "flex", width: "100%", alignItems: "center" }}>
        <Button
          onClick={handleOpenAdd}
          sx={{
            background: "var(--main-color-500)",
            color: "#fff",
            "&:hover": {
              backgroundColor: "var(--main-color-btn)",
            },
          }}
        >
          қосу
        </Button>
      </Box>
      <TableAdmin />
      <AddAdmin
        isOpenAddAdmin={isOpenAddAdmin}
        handleCloseAdd={handleCloseAdd}
      />
    </div>
  );
};

export default AddNewAdmmin;
