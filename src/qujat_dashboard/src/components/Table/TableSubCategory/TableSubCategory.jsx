import React, { useState } from "react";
import {
  Table,
  TableHead,
  TableRow,
  TableBody,
  TableCell,
  Box,
} from "@mui/material";

import { FcPlus } from "react-icons/fc";

import edit from "../../../assets/images/edit.svg";
import trash from "../../../assets/images/trash.svg";

import AddSubCategoryForm from "../../Modals/AddSubCategoryForm/AddSubcCategoryForm";
import UpdateSubCategory from "../../Modals/UpdateSubCategory/UpdateSubCategory";
import DeleteSubCategory from "../../Modals/DeleteSubCategory/DeleteSubCategory";

const TableSubCategory = ({ data }) => {
  const [isOpenModal, setIsOpenModal] = useState(false);
  const [isOpenUpdate, setIsOpenUpdate] = useState(false);
  const [isOpenDelete, setsIsOpenDelete] = useState(false);

  //ADD NEW ITEM HANDLER
  const handleOpen = () => {
    setIsOpenModal(true);
  };
  const handleClose = () => {
    setIsOpenModal(false);
  };

  //UPDATE CURRENT ITEM HANDLER
  const handleOpenUpdate = () => {
    setIsOpenUpdate(true);
  };

  const handleCloseUpdate = () => {
    setIsOpenUpdate(false);
  };

  //DELETE CURRENT ITEM HANDLER
  const handleOpenDelete = () => {
    setsIsOpenDelete(true);
  };

  const handleCloseDelete = () => {
    setsIsOpenDelete(false);
  };

  return (
    <Table>
      <TableHead>
        <TableRow>
          <TableCell>
            <h2>#</h2>
          </TableCell>
          <TableCell>
            <h2>Тақырыптын ішкі категорияның аты</h2>
          </TableCell>
          <TableCell>
            <h2>Иконка</h2>
          </TableCell>
          <TableCell>
            <h2>Әрекет</h2>
          </TableCell>
          <TableCell>
            <FcPlus
              style={{ fontSize: "32px", cursor: "pointer" }}
              onClick={handleOpen}
            />
          </TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {data?.map((item) => (
          <TableRow key={item.id} className="table_item">
            <TableCell>{item.id}</TableCell>
            <TableCell>{item.name}</TableCell>
            <TableCell>{item.date}</TableCell>
            <TableCell>
              <Box className="table_action">
                <img src={edit} onClick={handleOpenUpdate} />
                <img src={trash} onClick={handleOpenDelete} />
              </Box>
            </TableCell>
            <TableCell></TableCell>
          </TableRow>
        ))}
      </TableBody>
      <AddSubCategoryForm isOpenModal={isOpenModal} handleClose={handleClose} />
      <UpdateSubCategory
        isOpenUpdate={isOpenUpdate}
        handleCloseUpdate={handleCloseUpdate}
      />
      <DeleteSubCategory
        isOpenDelete={isOpenDelete}
        handleCloseDelete={handleCloseDelete}
      />
    </Table>
  );
};

export default TableSubCategory;
